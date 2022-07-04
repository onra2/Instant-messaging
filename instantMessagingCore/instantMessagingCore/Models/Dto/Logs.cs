using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace instantMessagingCore.Models.Dto
{
    public class Logs
    {
        /// <summary>
        /// A unique id for the entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The type of entry
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The entry message
        /// </summary>
        public string Message { get; set; }

        public Logs()
        {

        }

        public Logs(EType type, string message) : this(type.ToString(), message)
        {
        }

        public Logs(string type, string message)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public Logs(int id, EType type, string message) : this(id, type.ToString(), message)
        {
        }

        public Logs(int id, string type, string message)
        {
            Id = id;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public enum EType
        {
            error,
            warning,
            info
        }
    }
}
