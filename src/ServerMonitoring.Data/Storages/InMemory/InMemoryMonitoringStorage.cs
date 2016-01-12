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
        private readonly ConcurrentDictionary<string, ServerInfo> _servers;
        private readonly ConcurrentDictionary<string, InMemoryServerData> _serversData;

        public InMemoryMonitoringStorage()
        {
            _servers = new ConcurrentDictionary<string, ServerInfo>();
            _serversData = new ConcurrentDictionary<string, InMemoryServerData>();
        }

        public async Task<List<ServerInfo>> GetServersAsync()
        {
            return await Task.FromResult(_servers.Select(y => y.Value).ToList());
        }

        public async Task PushAsync(ServerPushData data)
        {
            if (data == null)
                return;
            
            _servers.AddOrUpdate(data.Server.Id, i => data.Server, (i, u) => data.Server);

            var serverData = _serversData.GetOrAdd(data.Server.Id, hash => new InMemoryServerData());

            await serverData.InternalPush(data.Items);
        }

        public async Task<ServerPullData> PullAsync(MonitoringQuery query)
        {
            var serverData = _serversData.GetOrAdd(query.Server.Id, hash => new InMemoryServerData()).ToList();

            if(!serverData.Any())
                return new ServerPullData();

            var result = new ServerPullData();

            foreach (var sData in serverData)
            {
                var item = new ServerStatisticsDataItem
                {
                    Name = sData.Key,
                    CurrentValue = await sData.Value.LastValue(),
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
