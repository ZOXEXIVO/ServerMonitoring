using System;
using System.Collections.Generic;
using System.Linq;
using ServerMonitoring.WindowsAgent.Models;
using ServerMonitoring.WindowsAgent.MonitoringServices;
using ServerMonitoring.WindowsAgent.MonitoringServices.CPU;
using ServerMonitoring.WindowsAgent.MonitoringServices.Disk;
using ServerMonitoring.WindowsAgent.MonitoringServices.Memory;
using ServerMonitoring.WindowsAgent.MonitoringServices.Network;
using ServerMonitoring.WindowsAgent.Services.ComputerID;
using ServerMonitoring.WindowsAgent.Services.ComputerID.MacIDService;

namespace ServerMonitoring.WindowsAgent.Application
{
    public class MonitoringApp
    {
        private readonly List<IMonitoringService> _monitoringServices;
        private readonly IComputerIdService _idService;

        private ServerInfo _serverInfo;

        public MonitoringApp()
        {
            _monitoringServices = new List<IMonitoringService>
            {
                new CpuMonitoringService(),
                new MemoryMonitoringService(),
                new DiskMonitoringService(),
                new NetworkMonitoringService()
            };

            _idService = new MacAddressIdService();

            InitServerInfo();
        }

        public void Start()
        {
            foreach (var service in _monitoringServices)
                service.Start();
        }

        public void Stop()
        {
            foreach (var service in _monitoringServices)
                service.Stop();
        }

        public ServerInfo Server => _serverInfo;

        private void InitServerInfo()
        {
            _serverInfo = new ServerInfo
            {
                Id = _idService.GetCurrentComputerId(),
                MachineName = Environment.MachineName
            };
        }

        public ServerPushData GetDataToPush()
        {
            return new ServerPushData()
            {
                Server = _serverInfo,
                Items = _monitoringServices.SelectMany(x => x.GetData())
            };
        }
    }
}
