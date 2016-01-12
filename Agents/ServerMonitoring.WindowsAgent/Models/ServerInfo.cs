using System.Text;

namespace ServerMonitoring.WindowsAgent.Models
{
    public class ServerInfo
    {
        /// <summary>
        /// Server name
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Server IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Is server active
        /// </summary>
        public bool IsActive { get; set; }

        public override int GetHashCode()
        {
            return MachineName.GetHashCode() ^ IP.GetHashCode();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendFormat("MachineName: {0}\r\n", MachineName);
            builder.AppendFormat("IP: {0}\r\n", IP);

            return builder.ToString();
        }
    }
}
