using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerMonitoring.Data.Collections;
using ServerMonitoring.Data.Enums;
using ServerMonitoring.Data.Models.Common;
using ServerMonitoring.Data.Models.Query;

namespace ServerMonitoring.Data.Storages.InMemory.Data
{
    public class InMemoryInternalData
    {
        private readonly FixedSizeConcurrentQueue<InMemoryTimeValue> _monitoringData;

        public int Order { get; set; }
        public string Display { get; set; }
        public ServerStatisticsType Type { get; set; }

        public InMemoryInternalData(ServerStatisticsType type)
        {
            Type = type;

            _monitoringData = new FixedSizeConcurrentQueue<InMemoryTimeValue>(86400 /* one day */);
        }

        public async Task InternalPush(ServerStatisticsDataItem item)
        {
            _monitoringData.Enqueue(new InMemoryTimeValue(item.CurrentValue, DateTime.UtcNow));

            Order = item.Order;
            Display = item.CurrentValueDisplay;

            await Task.CompletedTask;
        }

        public async Task<List<double>> FilterData(MonitoringQuery query)
        {
            IEnumerable<InMemoryTimeValue> data = _monitoringData;

            if (query.DateFrom.HasValue)
                data = data.Where(x => x.Date >= query.DateFrom);

            if (query.DateTo.HasValue)
                data = data.Where(x => x.Date <= query.DateTo);

            if (query.SinceDate.HasValue)
                data = data.Where(x => x.Date >= query.SinceDate);

            if (query.SinceMinute.HasValue)
            {
                var nowWithoutMinute = DateTime.UtcNow.AddMinutes(-query.SinceMinute.Value);
                data = data.Where(x => x.Date >= nowWithoutMinute);
            }

            //by default - 5 minutes
            if (query.IsEmpty)
            {
                var defaultLastTime = DateTime.UtcNow.AddMinutes(-5);
                data = data.Where(x => x.Date > defaultLastTime);
            }

            return await Task.FromResult(data.Select(y => y.Value).ToList());
        }

        public async Task<double> LastValue()
        {
            if (!_monitoringData.Any())
                return 0.0;

            return await Task.FromResult(_monitoringData.LastOrDefault().Value);
        }
    }
}
