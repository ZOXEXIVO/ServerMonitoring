using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerMonitoring.Data.Models;
using ServerMonitoring.Data.Models.Common;
using ServerMonitoring.Data.Models.Query;
using ServerMonitoring.Data.Storages.InMemory.Data;

namespace ServerMonitoring.Data.Storages.InMemory
{
    public class InMemoryMonitoringStorage : IMonitoringStorage
    {
        private readonly ConcurrentDictionary<int, ServerInfo> _servers;
        private readonly ConcurrentDictionary<int, InMemoryServerData> _serversData;

        public InMemoryMonitoringStorage()
        {
            _servers = new ConcurrentDictionary<int, ServerInfo>();
            _serversData = new ConcurrentDictionary<int, InMemoryServerData>();
        }

        public async Task<List<ServerInfo>> GetServersAsync()
        {
            return await Task.FromResult(_servers.Select(y => y.Value).ToList());
        }

        public async Task PushAsync(ServerPushData data)
        {
            if (data == null)
                return;

            var serverData = _serversData.GetOrAdd(data.Server.GetHashCode(), hash => new InMemoryServerData());

            await serverData.InternalPush(data.Items);
        }

        public async Task<ServerPullData> PullAsync(MonitoringQuery query)
        {
            var serverHash = query.Server.GetHashCode();

            var serverData = _serversData.GetOrAdd(serverHash, hash => new InMemoryServerData()).ToList();

            if(!serverData.Any())
                return new ServerPullData();

            var result = new ServerPullData();

            foreach (var sData in serverData)
            {
                var item = new ServerStatisticsDataItem
                {
                    Name = sData.Key,
                    CurrentValueDisplay = sData.Value.Display,
                    Type = sData.Value.Type,
                    Data = await sData.Value.FilterData(query)
                };

                result.Items.Add(item);
            }

            return await Task.FromResult(result);
        }
    }
}
