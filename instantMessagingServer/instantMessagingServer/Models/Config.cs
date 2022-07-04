using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace instantMessagingServer.Models
{
    public static class Config
    {

        private static IConfiguration _Configuration;

        /// <summary>
        /// Acces to the configuration file
        /// </summary>
        public static IConfiguration Configuration
        {
            get { return _Configuration; }
            set
            {
                if (_Configuration == null)
                {
                    _Configuration = value;
                }
            }
        }

    }
}
