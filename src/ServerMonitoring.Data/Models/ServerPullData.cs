using System.Collections.Generic;
using ServerMonitoring.Data.Models.Common;

namespace ServerMonitoring.Data.Models
{
    /// <summary>
    /// Model came from Server
    /// </summary>
    public class ServerPullData
    {
        public ServerPullData()
        {
            Items = new List<ServerStatisticsDataItem>();
        }

        /// <summary>
        /// Data items (CPU, Memory, Disk data)
        /// </summary>
        public List<ServerStatisticsDataItem> Items { get; set; } 
    }
}
