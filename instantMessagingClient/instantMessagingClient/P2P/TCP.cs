using System;
using System.Net;
using System.Text;
using EasyConsoleApplication;
using instantMessagingClient.Database;
using instantMessagingClient.JsonRest;
using instantMessagingClient.Model;
using instantMessagingClient.Pages;
using instantMessagingCore.Crypto;
using SimpleTCP;

namespace instantMessagingClient.P2P
{
    public class TCP
    {
        //access to our database
        public DatabaseContext db { get; set; }

        //our ip address
        public string myHost { get; set; }

        public string myPort { get; set; }

        //the ip address of the friend
        public string friendsHost { get; set; }

        public string friendsPort { get; set; }

        //our tcp client to send messages
        public SimpleTcpClient myClient { get; set; }

        //the tcp server to receive messages
        public SimpleTcpServer myServer { get; set; } 

        private readonly ChatManager cm = ChatManager.getInstance();

        public TCP(string myHost, string myPort)
        {
            this.myHost = myHost;
            this.myPort = myPort;//8910
            db = new DatabaseContext();
            db.Database.EnsureCreated();
        }

        public void Deconstruct(out SimpleTcpClient myClient, out SimpleTcpServer myServer)
        {
            myClient = this.myClient;
            myServer = this.myServer;

            myClient.Disconnect();
            myServer.Stop();
        }

        /// <summary>
        /// Starts listening to incoming messages
        /// </summary>
        public bool startListener()
        {
            myServer = new SimpleTcpServer { Delimiter = 0x13, StringEncoder = Encoding.UTF8 };
            myServer.DataReceived += Message_Received;

            IPAddress ip = IPAddress.Parse(this.myHost);
            myServer.Stop();
            myServer.Start(ip, Convert.ToInt32(this.myPort));
            return myServer.IsStarted;
        }

        /// <summary>
        /// starts the client to be able to send messages
        /// </summary>
        public void startClient()
        {
            myClient = new SimpleTcpClient { StringEncoder = Encoding.UTF8 };
            try
            {
                myClient.Connect(this.friendsHost, Convert.ToInt32(this.friendsPort));
            }
            catch (Exception e)
            {
                ConsoleHelpers.WriteRed("Your friend isn't online.");
                ConsoleHelpers.WriteRed(e.Message);
                ConsoleHelpers.HitEnterToContinue();
                Application.GoTo<FriendList>();
                throw;
            }
        }

        /// <summary>
        /// On message received, add it to the local db
        /// </summary>
        private void Message_Received(object sender, Message e)
        {
            MyMessages msg = e.MessageString.Deserialize<MyMessages>();
            
            this.db.MyMessages.Add(msg);
            this.db.SaveChanges();
            cm.AskUpdate(msg.IdEnvoyeur);
        }

        /// <summary>
        /// On message send, encrypt it, add it to the local db and send it
        /// </summary>
        public void sendMessage(MyMessages msg, postKey friendKey)
        {
            RSAManager rsaFriend = new RSAManager(friendKey.Key);
            RSAManager myRSA = new RSAManager(Session.maKey.Key);

            byte[] text = Encoding.ASCII.GetBytes(msg.message);
            text = rsaFriend.Encrypt(text);

            byte[] mytext = Encoding.ASCII.GetBytes(msg.message);
            mytext = myRSA.Encrypt(mytext);


            string encodedStr = Convert.ToBase64String(text);
            string myencodedStr = Convert.ToBase64String(mytext);

            msg.message = encodedStr;

            string toSend = msg.Serialize();
            try
            {
                myClient.Write(toSend);
                //copy pour ma bd
                MyMessages myMsg = new MyMessages(msg.IdEnvoyeur, myencodedStr);
                this.db.MyMessages.Add(myMsg);
                this.db.SaveChanges();
            }
            catch (Exception e)
            {
                ConsoleHelpers.WriteRed("Your friend disconnected.");
                ConsoleHelpers.WriteRed(e.Message);
                ConsoleHelpers.HitEnterToContinue();
                myClient.Disconnect();
                Application.GoBack();
            }
            
            cm.AskUpdate(Session.tokens.UserId);
        }
    }
}
