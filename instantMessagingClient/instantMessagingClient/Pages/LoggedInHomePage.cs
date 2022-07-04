using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using EasyConsoleApplication;
using EasyConsoleApplication.Menus;
using EasyConsoleApplication.Pages;
using instantMessagingClient.Model;
using instantMessagingClient.P2P;
using instantMessagingCore.Models.Dto;

namespace instantMessagingClient.Pages
{
    public class LoggedInHomePage : Page
    {
        /// <summary>
        /// Gets our public IP Address
        /// </summary>
        /// <returns>IPAddress</returns>
        public static IPAddress GetMyIp()
        {
            List<string> services = new List<string>()
            {
                "https://ipv4.icanhazip.com",
                "https://api.ipify.org",
                "https://ipinfo.io/ip",
                "https://checkip.amazonaws.com",
                "https://wtfismyip.com/text",
                "http://icanhazip.com"
            };
            using var webclient = new WebClient();
            foreach (var service in services)
            {
                try { return IPAddress.Parse(webclient.DownloadString(service)); } catch { }
            }
            return null;
        }

        /// <summary>
        /// Gets our local IP
        /// </summary>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public LoggedInHomePage()
        {
            Title = "Home: " + Session.sessionUsername + "(" + Session.tokens.UserId + ")";
            TitleColor = ConsoleColor.Green;
            Body = "-----";
            MenuItems.Add(new MenuItem("Friends list", clickFriendsList));
            MenuItems.Add(new MenuItem("Disconnect", clickDisconnect)
            {
                Color = ConsoleColor.Yellow
            });

            //If we just logged in, post our peers info to the server and start listening to incoming messages
            //start the heartbeat
            if (Session.hasAlreadyStarted == false)
            {
                Rest rest = new Rest();
                string Localipv4 = GetLocalIPAddress();
                IPAddress ipv6 = GetMyIp();
                Random random = new Random();
                var myPort = Convert.ToUInt16(random.Next(49153, 65534));
                //var param = new Peers(Session.tokens.UserId, Localipv4, ipv6.ToString(), myPort, DateTime.Now);
                var param = new Peers(Session.tokens.UserId, Localipv4, Localipv4, myPort, DateTime.Now);
                rest.postPeers(param);
                Session.communication = new TCP(Localipv4, Convert.ToString((int)myPort));
                Session.communication.startListener();
                Session.hasAlreadyStarted = true;
                Heartbeat.getInstance().start();
            }
        }

        private void clickFriendsList()
        {
            Application.GoTo<FriendList>();
        }

        /// <summary>
        /// Disconnects
        /// </summary>
        public static void clickDisconnect()
        {
            Session.sessionPassword = null;
            Session.sessionUsername = null;
            Session.tokens = null;
            Session.communication = null;
            Session.isOnMessagingPage = false;
            Session.maKey = null;
            Session.hasAlreadyStarted = false;
            Heartbeat.getInstance().stop();
            Application.GoTo<Home>();
        }
    }
}
