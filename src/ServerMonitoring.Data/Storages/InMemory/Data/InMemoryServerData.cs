using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServerMonitoring.Data.Models.Common;

namespace ServerMonitoring.Data.Storages.InMemory.Data
{
    public class InMemoryServerData : ConcurrentDictionary<string, InMemoryInternalData>
    {
        public async Task InternalPush(List<ServerStatisticsDataItem> items)
        {
            foreach (var item in items)
            {
                var data = GetOrAdd(item.Name, name => new InMemoryInternalData(item.Type));

                await data.InternalPush(item);
            }
        }
    }
}
