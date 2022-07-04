using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace instantMessagingClient.Database
{
    //Stores the private key in the local database
    public class MyKey
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Key { get; set; }

        public MyKey()
        {
        }

        public MyKey(int userId, string key)
        {
            UserId = userId;
            Key = key;
        }

        public MyKey(int id, int userId, string key)
        {
            Id = id;
            UserId = userId;
            Key = key;
        }
    }
}
