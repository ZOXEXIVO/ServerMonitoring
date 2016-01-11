﻿using System;
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

            if (query.SinceByDate.HasValue)
                data = data.Where(x => x.Date > query.SinceByDate);

            return await Task.FromResult(data.Select(y => y.Value).ToList());
        } 
    }
}