using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using ServerMonitoring.WindowsAgent.Models;
using ServerMonitoring.WindowsAgent.MonitoringServices;
using ServerMonitoring.WindowsAgent.MonitoringServices.CPU;
using ServerMonitoring.WindowsAgent.MonitoringServices.Disk;
using ServerMonitoring.WindowsAgent.MonitoringServices.Memory;
using ServerMonitoring.WindowsAgent.MonitoringServices.Network;
using ServerMonitoring.WindowsAgent.Services.ComputerID;
using ServerMonitoring.WindowsAgent.Services.ComputerID.MacIDService;
using ServerMonitoring.WindowsAgent.Services.CPU;
using ServerMonitoring.WindowsAgent.Services.CPU.WMI;
using ServerMonitoring.WindowsAgent.Services.Output;
using ServerMonitoring.WindowsAgent.Services.Output.ConsoleOutput;
using ServerMonitoring.WindowsAgent.Services.Transport;
using ServerMonitoring.WindowsAgent.Services.Transport.HttpTransport;

namespace ServerMonitoring.WindowsAgent.Application
{
    public class MonitoringApp
    {
        private readonly List<IMonitoringService> _monitoringServices;
        private readonly IComputerIdService _idService;
        private readonly ITransportService _transportService;
        private readonly IOutputService _outputService;
        private readonly ICpuNameService _cpuNameService;

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
            _transportService = new HttpTransportService();
            _outputService = new ConsoleOutputService();
            _cpuNameService = new WMICpuNameService();

            InitServerInfo();
        }

        public void Start()
        {
            _outputService.InfoLine($"Monitoring started for {_serverInfo.MachineName}");
            _outputService.InfoLine(_transportService.GetInfo());

            foreach (var service in _monitoringServices)
                service.Start();

            while (true)
            {
                var pushData = GetDataToPush();

                try
                {
                    _outputService.Info("pushing... ");

                    var elapsedTimeTask = _transportService.PushAsync(pushData);

                    elapsedTimeTask.Wait();

                    _outputService.InfoLine("ok. " + elapsedTimeTask.Result + " ms");

                    Thread.Sleep(1000);
                }
                catch (WebException ex)
                {
                    _outputService.InfoLine("webexception");
                    _outputService.InfoLine(ex.Message);

                    Thread.Sleep(3000);
                }
                catch (Exception ex)
                {
                    _outputService.InfoLine("exception");
                    _outputService.InfoLine(ex.Message);

                    Thread.Sleep(3000);
                }
            }
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
                MachineName = Environment.MachineName,
                Processors = _cpuNameService.GetCPUNames()
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
