using System.Collections.Generic;
using System.Linq;

namespace ServerMonitoring.Data.Models.Common
{
    public class ServerInfo
    {
        public ServerInfo()
        {
            Processors = Enumerable.Empty<string>();
        }

        /// <summary>
        /// Server ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Server name
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// CPUs
        /// </summary>
        public IEnumerable<string> Processors { get; set; } 

        /// <summary>
        /// Is server active
        /// </summary>
        public bool IsActive { get; set; }
    }
}
