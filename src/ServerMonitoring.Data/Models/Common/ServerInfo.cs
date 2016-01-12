using System.Collections.Generic;

namespace ServerMonitoring.Data.Models.Common
{
    public class ServerInfo
    {
        public ServerInfo()
        {
            IPs = new List<string>();    
        }

        /// <summary>
        /// Server name
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Server IP
        /// </summary>
        public List<string> IPs { get; set; }

        /// <summary>
        /// Is server active
        /// </summary>
        public bool IsActive { get; set; }

        public override int GetHashCode()
        {
            return MachineName.GetHashCode() ^ string.Join(", ", IPs).GetHashCode();
        }
    }
}
