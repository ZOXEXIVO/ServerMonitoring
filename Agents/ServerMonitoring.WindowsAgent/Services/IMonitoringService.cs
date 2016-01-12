using System.Collections.Generic;
using ServerMonitoring.WindowsAgent.Models;

namespace ServerMonitoring.WindowsAgent.Services
{
    public interface IMonitoringService
    {
        void Start();
        void Stop();

        IEnumerable<ServerStatisticsDataItem> GetData();
    }
}
