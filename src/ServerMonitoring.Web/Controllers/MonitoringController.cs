using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ServerMonitoring.Data.Models;
using ServerMonitoring.Data.Models.Query;
using ServerMonitoring.Data.Storages;
using ServerMonitoring.Web.Application.Formatters.Bindings;

namespace ServerMonitoring.Web.Controllers
{
    public class MonitoringController : Controller
    {
        private readonly IMonitoringStorage _monitoringStorage;

        public MonitoringController(IMonitoringStorage monitoringStorage)
        {
            _monitoringStorage = monitoringStorage;
        }

        [HttpGet]
        public async Task<IActionResult> GetServers()
        {
            var result = await _monitoringStorage.GetServersAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Push(ServerPushData data)
        {
            await _monitoringStorage.PushAsync(data);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Pull([FromJsonUri]MonitoringQuery query)
        {
            var result = await _monitoringStorage.PullAsync(query);
            return Ok(result);
        }
    }
}
