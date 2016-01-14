using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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

        /// <summary>
        /// Last server push
        /// </summary>
        [JsonIgnore]
        public DateTime? LastPush { get; set; }
    }
}
