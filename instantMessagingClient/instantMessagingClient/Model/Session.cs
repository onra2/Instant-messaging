using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using instantMessagingClient.Database;
using instantMessagingClient.P2P;
using instantMessagingCore.Models.Dto;
using Newtonsoft.Json;

namespace instantMessagingClient.Model
{
    //session class used to access data using in the current user session
    public static class Session
    {
        //contains the token & user id of the session
        public static Tokens tokens { get; set; }

        public static string sessionUsername { get; set; }

        public static SecureString sessionPassword { get; set; }

        public static bool isOnMessagingPage { get; set; }

        //the communication object
        public static TCP communication { get; set; }

        //our private key
        public static MyKey maKey { get; set; }

        public static bool hasAlreadyStarted { get; set; }
    }
}
