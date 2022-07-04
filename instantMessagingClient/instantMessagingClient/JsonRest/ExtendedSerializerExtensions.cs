using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace instantMessagingClient.JsonRest
{
    public static class ExtendedSerializerExtensions
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        public static string Serialize<T>(this T source)
        {
            var asString = JsonConvert.SerializeObject(source, SerializerSettings);
            return asString;
        }

        public static T Deserialize<T>(this string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }
    }
}
