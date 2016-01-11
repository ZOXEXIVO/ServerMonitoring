﻿using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ServerMonitoring.Data.Models;
using ServerMonitoring.Data.Models.Query;
using ServerMonitoring.Data.Storages;

namespace ServerMonitoring.Web.Controllers
{
    public class MonitoringController : Controller
    {
        private readonly IMonitoringStorage _monitoringStorage;

        public MonitoringController(IMonitoringStorage monitoringStorage)
        {
            _monitoringStorage = monitoringStorage;
        }

        public async Task<IActionResult> Push(ServerPushData data)
        {
            await _monitoringStorage.PushAsync(data);
            return Ok();
        }

        public async Task<IActionResult> Pull(MonitoringQuery query)
        {
            var result = await _monitoringStorage.PullAsync(query);
            return Ok(result);
        }
    }
}