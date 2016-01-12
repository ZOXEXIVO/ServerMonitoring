using System;
using System.Threading;
using ServerMonitoring.WindowsAgent.Application;
using ServerMonitoring.WindowsAgent.Pusher;

namespace ServerMonitoring.WindowsAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            var application = new MonitoringApp();

            application.Start();

            while (true)
            {
                var pushData = application.GetDataToPush();

                var pusher = new HttpPusher();

                pusher.Push(pushData);

                Thread.Sleep(1000);
            }
        }
    }
}
