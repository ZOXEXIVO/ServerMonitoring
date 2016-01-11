using System.Collections.Generic;
using System.Threading.Tasks;
using ServerMonitoring.Data.Models;
using ServerMonitoring.Data.Models.Common;
using ServerMonitoring.Data.Models.Query;

namespace ServerMonitoring.Data.Storages
{
    public interface IMonitoringStorage
    {
        /// <summary>
        /// Update monitoring info from server
        /// </summary>
        /// <returns></returns>
        Task<List<ServerInfo>> GetServers();

        /// <summary>
        /// Update monitoring info from server
        /// </summary>
        /// <param name="serverData"></param>
        /// <returns></returns>
        Task PushAsync(ServerPushData serverData);

        /// <summary>
        /// Get monitoring info for server
        /// </summary>
        /// <param name="query">query data</param>
        /// <returns></returns>
        Task<ServerPullData> PullAsync(MonitoringQuery query);
    }
}
