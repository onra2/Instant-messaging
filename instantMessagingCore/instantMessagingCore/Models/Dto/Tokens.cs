using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace instantMessagingCore.Models.Dto
{
    public class Tokens
    {
        /// <summary>
        /// The owner id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The owner connexion token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The token expiration date
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        public Tokens(int userId, string token, DateTime expirationDate)
        {
            UserId = userId;
            Token = token ?? throw new ArgumentNullException(nameof(token));
            this.ExpirationDate = expirationDate;
        }
    }
}
