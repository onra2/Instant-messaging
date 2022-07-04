using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyConsoleApplication;
using instantMessagingClient.Database;
using instantMessagingClient.JsonRest;
using instantMessagingCore.Crypto;
using instantMessagingCore.Models.Dto;

namespace instantMessagingClient.Model
{
    public class CurrentChat
    {
        public bool isRunning { get; set; }

        public DatabaseContext db { get; set; }

        public int friendID { get; set; }

        //defines what string has to be entered to go back a page in the menus
        public string backCommand { get; set; }

        //the public key of a friend
        public postKey pkFriend { get; set; }
        
        //our rsa to encrypt and decrypt messages
        public RSAManager myManager { get; set; }

        public string friendName { get; set; }

        public CurrentChat(int friendID, string backCommand, postKey pkFriend, string friendName)
        {
            this.friendID = friendID;
            this.backCommand = backCommand;
            this.db = new DatabaseContext();
            db.Database.EnsureCreated();
            this.pkFriend = pkFriend;
            this.myManager = new RSAManager(Session.maKey.Key);
            this.friendName = friendName;
        }

        /// <summary>
        /// Reads the console to send a message
        /// </summary>
        public void readLine()
        {
            isRunning = true;
            Task.Factory.StartNew(() =>
            {
                string rawString;
                do
                {
                    rawString = ConsoleHelpers.Readline(ConsoleColor.White, "You: ");
                    //don't accept null or empty
                    if (string.IsNullOrEmpty(rawString) || rawString == backCommand) continue;
                    
                    MyMessages msg = new MyMessages(Session.tokens.UserId, rawString);
                    Session.communication.sendMessage(msg, pkFriend);
                    
                    Console.SetCursorPosition(0, ((Console.CursorTop > 0) ? Console.CursorTop - 1 : 0));
                    Console.Write("".PadRight(Console.BufferWidth));
                } while (rawString != backCommand);
                isRunning = false;
            });
        }

        /// <summary>
        /// Refreshes the console when a new message is received
        /// </summary>
        public void display()
        {
            Console.SetCursorPosition(0, 0);
            ConsoleHelpers.WriteGreen("If you want to go back, type '" + backCommand + "'");

            this.db.MyMessages.ToList().ForEach(m =>
            {
                if (m.IdEnvoyeur == Session.tokens.UserId)
                {
                    Console.Write("You said: ");
                    var text = this.myManager.Decrypt(Convert.FromBase64String(m.message));
                    Console.WriteLine(Encoding.UTF8.GetString(text));
                }
                else
                {
                    Console.Write(this.friendName + " said: ");
                    var text = this.myManager.Decrypt(Convert.FromBase64String(m.message));
                    Console.WriteLine(Encoding.UTF8.GetString(text));
                }
            });
        }
    }
}
