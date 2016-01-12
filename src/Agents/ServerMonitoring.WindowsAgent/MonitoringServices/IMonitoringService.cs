using System.Collections.Generic;
using ServerMonitoring.WindowsAgent.Models;

namespace ServerMonitoring.WindowsAgent.MonitoringServices
{
    public interface IMonitoringService
    {
        void Start();
        void Stop();

        IEnumerable<ServerStatisticsDataItem> GetData();
    }
}
