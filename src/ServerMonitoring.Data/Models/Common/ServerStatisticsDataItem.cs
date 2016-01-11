using ServerMonitoring.Data.Enums;

namespace ServerMonitoring.Data.Models.Common
{
    public class ServerStatisticsDataItem
    {
        /// <summary>
        /// Name of item (CPU_1, DISK_READ e.t.c)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Current value (40.0 - CPU load 40%, DISK read % e.t.c)
        /// </summary>
        public double CurrentValue { get; set; }

        /// <summary>
        /// Current value dimension (%, KB, B e.t.c)
        /// </summary>
        public string CurrentValueDisplay { get; set; }

        /// <summary>
        /// Type of element for grouping
        /// </summary>
        public ServerStatisticsType Type { get; set; }

        /// <summary>
        /// Data for last 
        /// </summary>
        public double[] Data { get; set; }
    }
}
