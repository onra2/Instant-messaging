using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace instantMessagingServer.Models.Api
{
    public class KeyValueData
    {

        public string Key { get; set; }
        public byte[] Value { get; set; }

        public KeyValueData()
        {

        }

        public KeyValueData(string key, byte[] value)
        {
            this.Key = key ?? throw new ArgumentNullException(nameof(key));
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
