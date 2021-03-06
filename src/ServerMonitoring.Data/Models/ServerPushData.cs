﻿using System.Collections.Generic;
using ServerMonitoring.Data.Models.Common;

namespace ServerMonitoring.Data.Models
{
    /// <summary>
    /// Model came from Server
    /// </summary>
    public class ServerPushData
    {
        public ServerPushData()
        {
            Items = new List<ServerStatisticsDataItem>();
        }

        /// <summary>
        /// Server info
        /// </summary>
        public ServerInfo Server { get; set; }

        /// <summary>
        /// Data items (CPU, Memory, Disk data)
        /// </summary>
        public List<ServerStatisticsDataItem> Items { get; set; } 
    }
}
