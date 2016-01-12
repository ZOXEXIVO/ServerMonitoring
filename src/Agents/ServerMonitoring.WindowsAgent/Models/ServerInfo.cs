using System;
using System.Collections.Generic;
using System.Text;

namespace ServerMonitoring.WindowsAgent.Models
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

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendFormat("MachineName: {0}\r\n", MachineName);
            builder.AppendLine("IPs:");

            foreach (var ip in IPs)
                builder.AppendLine(ip);
            
            return builder.ToString();
        }
    }
}
