using System;
using ServerMonitoring.Data.Models.Common;

namespace ServerMonitoring.Data.Models.Query
{
    public class MonitoringQuery
    {
        /// <summary>
        /// Server info
        /// </summary>
        public ServerInfo Server { get; set; }

        /// <summary>
        /// Start datetime
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// End datetime
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// After record id
        /// </summary>
        public DateTime? SinceByDate { get; set; }


        public bool IsEmpty => !DateFrom.HasValue && !DateTo.HasValue && !SinceByDate.HasValue;
    }
}
