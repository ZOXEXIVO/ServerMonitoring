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
        private readonly ConcurrentDictionary<string, DateTime> _serversLastPush;
        private readonly ConcurrentDictionary<string, InMemoryServerData> _serversData;

        public InMemoryMonitoringStorage()
        {
            _servers = new ConcurrentDictionary<string, ServerInfo>();
            _serversLastPush = new ConcurrentDictionary<string, DateTime>();
            _serversData = new ConcurrentDictionary<string, InMemoryServerData>();
        }

        public async Task<List<ServerInfo>> GetServersAsync()
        {
            foreach (var server in _servers.Select(y => y.Value))
            {
                var lastPushDate = _serversLastPush.GetOrAdd(server.Id, hash => DateTime.UtcNow);
                server.IsActive = (DateTime.UtcNow - lastPushDate).TotalSeconds < 10;
            }

            return await Task.FromResult(_servers.Select(y => y.Value).ToList());
        }

        public async Task PushAsync(ServerPushData data)
        {
            if (data == null)
                return;
            
            _servers.AddOrUpdate(data.Server.Id, i => data.Server, (i, u) => data.Server);

            _serversLastPush.AddOrUpdate(data.Server.Id, i => DateTime.UtcNow, (i, u) => DateTime.UtcNow);

            var serverData = _serversData.GetOrAdd(data.Server.Id, hash => new InMemoryServerData());

            await serverData.InternalPush(data.Items);
        }

        public async Task<ServerPullData> PullAsync(MonitoringQuery query)
        {
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

            var result = new ServerPullData { Items = items.OrderBy(x => Guid.NewGuid()).Take(1) };

            return await Task.FromResult(result);
        }
    }
}
