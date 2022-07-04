using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace instantMessagingCore.Models.Dto
{
    public class PublicKeys
    {
        /// <summary>
        /// The owner id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The data byte array of the key
        /// </summary>
        public byte[] Key { get; set; }

        /// <summary>
        /// The reference date
        /// </summary>
        public DateTime ValueDate { get; set; }

        public PublicKeys(int userId, byte[] key, DateTime valueDate)
        {
            UserId = userId;
            this.Key = key ?? throw new ArgumentNullException(nameof(key));
            this.ValueDate = valueDate;
        }
    }
}
