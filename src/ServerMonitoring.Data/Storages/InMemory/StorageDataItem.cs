using System;
using ServerMonitoring.Data.Enums;

namespace ServerMonitoring.Data.Storages.InMemory
{
    public class StorageDataItem
    {
        /// <summary>
        /// Name of item (CPU_1, DISK_READ e.t.c)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Current value (40.0 - CPU load 40%, DISK read % e.t.c)
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Current value dimension (%, KB, B e.t.c)
        /// </summary>
        public string Display { get; set; }

        /// <summary>
        /// Type of element for grouping
        /// </summary>
        public ServerStatisticsType Type { get; set; }

        /// <summary>
        /// Data for item
        /// </summary>
        public DateTime UtcDate { get; set; }
    }
}
