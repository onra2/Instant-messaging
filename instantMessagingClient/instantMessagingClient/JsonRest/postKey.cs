using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace instantMessagingClient.JsonRest
{
    public class postKey
    {
        public int userId { get; set; }

        /// <summary>
        /// The data byte array of the key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The reference date
        /// </summary>
        public DateTime valueDate { get; set; }

        public postKey(int userId, string key, DateTime valueDate)
        {
            this.userId = userId;
            Key = key;
            this.valueDate = valueDate;
        }

        public void toBase64()
        {
            Key = Convert.ToBase64String(Encoding.UTF8.GetBytes(Key));
        }

        public void ToStringNormal()
        {
            Key = Encoding.UTF8.GetString(Convert.FromBase64String(Key));
        }
    }
}
