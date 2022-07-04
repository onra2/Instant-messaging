using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace instantMessagingServer.Models
{
    public class Authentication
    {
        // Singleton
        private static Authentication authentication;

        public static Authentication GetInstance() => authentication ??= new Authentication();

        private Authentication()
        {
        }

        // Instance

        /// <summary>
        /// Check if the username correspond to the token
        /// </summary>
        /// <param name="name">the username to check</param>
        /// <param name="ClaimIDToken">the token to check</param>
        /// <returns>true if is autenticate</returns>
        public bool isAutheticate(string name, Claim ClaimIDToken)
        {
            DatabaseContext db = new(Config.Configuration);
            var user = db.Users.FirstOrDefault(u => u.Username == name);

            return user != null && db.Tokens.Any(t => t.Token == ClaimIDToken.Value && t.UserId == user.Id);
        }

        /// <summary>
        /// Generate a new unique IDToken
        /// </summary>
        /// <returns>the new unique IDToken</returns>
        public string GetIDToken()
        {
            DatabaseContext db = new(Config.Configuration);

            string IDToken;
            do
            {
                IDToken = Guid.NewGuid().ToString();
            } while (db.Tokens.Any(t => t.Token == IDToken));

            return IDToken;
        }
    }
}
