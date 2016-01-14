using System;
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
            foreach (var server in _servers.Select(y => y.Value))
            {
                if (!server.LastPush.HasValue)
                {
                    server.IsActive = false;
                    continue;
                }
                    
                server.IsActive = (DateTime.UtcNow - server.LastPush.Value).TotalSeconds < 10;
            }

            return await Task.FromResult(_servers.Select(y => y.Value).ToList());
        }

        public async Task PushAsync(ServerPushData data)
        {
            DateTime now = DateTime.UtcNow;

            if (data == null)
                return;
            
            var serverInfo = _servers.AddOrUpdate(data.Server.Id, i => data.Server, (i, u) => data.Server);

            serverInfo.LastPush = now;

            await _serversData.GetOrAdd(data.Server.Id, hash => new InMemoryServerData()).InternalPush(data.Items);
        }

        public async Task<ServerPullData> PullAsync(MonitoringQuery query)
        {
            var server = _servers.GetOrAdd(query.Server.Id, hash => new ServerInfo());

            var serverData = _serversData.GetOrAdd(query.Server.Id, hash => new InMemoryServerData()).ToList();

            if(!serverData.Any())
                return new ServerPullData();

            var items = new List<ServerStatisticsDataItem>();

            foreach (var sData in serverData)
            {
                var item = new ServerStatisticsDataItem
                {
                    Name = sData.Key,
                    Order = sData.Value.Order,
                    CurrentValue = await sData.Value.LastValue(),
                    CurrentValueDisplay = sData.Value.Display,
                    Type = sData.Value.Type,
                    Data = await sData.Value.FilterData(query)
                };

                items.Add(item);
            }

            var result = new ServerPullData { LastPush = server.LastPush, Items = items.OrderBy(x => x.Order) };

            return await Task.FromResult(result);
        }
    }
}
