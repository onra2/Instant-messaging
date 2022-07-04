using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace instantMessagingCore.Models.Dto
{
    public class Peers
    {
        /// <summary>
        /// The owner id
        /// </summary>
        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }

        /// <summary>
        /// The owner ip version 4
        /// </summary>
        public string Ipv4 { get; set; }

        /// <summary>
        /// The owner ip version 6
        /// </summary>
        public string Ipv6 { get; set; }

        /// <summary>
        /// The ower listening port
        /// </summary>
        [Required(ErrorMessage = "Port is required")]
        public ushort Port { get; set; }

        /// <summary>
        /// The last heartbeat receive from the client
        /// </summary>
        public DateTime LastHeartBeat { get; set; }

        public Peers(int userId, string ipv4, string ipv6, ushort port, DateTime lastHeartBeat)
        {
            if(ipv4 == null && ipv6 == null)
            {
                throw new ArgumentNullException($"{nameof(ipv4)} or {nameof(ipv6)} is mandatory");
            }
            UserId = userId;
            Ipv4 = ipv4;
            Ipv6 = ipv6;
            Port = port;
            LastHeartBeat = lastHeartBeat;
        }
    }
}
