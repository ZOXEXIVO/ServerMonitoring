using System.Text;

namespace ServerMonitoring.WindowsAgent.Models
{
    public class ServerInfo
    {
        /// <summary>
        /// Server ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Server name
        /// </summary>
        public string MachineName { get; set; }
        
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendFormat("MachineName: {0}\r\n", MachineName);
            builder.AppendFormat("ID: {0}\r\n", Id);

            return builder.ToString();
        }
    }
}
