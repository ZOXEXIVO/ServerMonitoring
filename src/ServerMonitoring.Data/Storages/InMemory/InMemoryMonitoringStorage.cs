using System.Collections.Generic;
using System.Threading.Tasks;
using ServerMonitoring.Data.Models;
using ServerMonitoring.Data.Models.Common;
using ServerMonitoring.Data.Models.Query;

namespace ServerMonitoring.Data.Storages.InMemory
{
    public class InMemoryMonitoringStorage : IMonitoringStorage
    {
        public Task<List<ServerInfo>> GetServers()
        {
            throw new System.NotImplementedException();
        }

        public Task PushAsync(ServerPushData serverData)
        {
            throw new System.NotImplementedException();
        }

        public Task<ServerPullData> PullAsync(MonitoringQuery query)
        {
            throw new System.NotImplementedException();
        }
    }
}
