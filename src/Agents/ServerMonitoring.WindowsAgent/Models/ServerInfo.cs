using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerMonitoring.WindowsAgent.Models
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

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendFormat("MachineName: {0}\r\n", MachineName);
            builder.AppendFormat("ID: {0}\r\n", Id);

            foreach (var cpu in Processors)
                builder.AppendLine(cpu);

            return builder.ToString();
        }
    }
}
