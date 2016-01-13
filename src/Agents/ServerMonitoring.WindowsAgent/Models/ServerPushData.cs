using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerMonitoring.WindowsAgent.Models
{
    /// <summary>
    /// Model came from Server
    /// </summary>
    public class ServerPushData
    {
        public ServerPushData()
        {
            Items = Enumerable.Empty<ServerStatisticsDataItem>();
        }

        /// <summary>
        /// Server info
        /// </summary>
        public ServerInfo Server { get; set; }

        /// <summary>
        /// Data items (CPU, Memory, Disk data)
        /// </summary>
        public IEnumerable<ServerStatisticsDataItem> Items { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine(Server.ToString());

            foreach (var item in Items)
                builder.AppendLine(item.ToString());

            return builder.ToString();
        }
    }
}
