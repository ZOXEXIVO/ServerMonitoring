using System;
using System.Collections.Generic;
using System.Linq;
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
            Items = Enumerable.Empty<ServerStatisticsDataItem>();
        }

        /// <summary>
        /// Last pushed data time
        /// </summary>
        public DateTime? LastPush { get; set; }

        /// <summary>
        /// Data items (CPU, Memory, Disk data)
        /// </summary>
        public IEnumerable<ServerStatisticsDataItem> Items { get; set; } 
    }
}
