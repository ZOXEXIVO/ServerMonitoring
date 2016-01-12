using System;
using System.Net;
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

            var serverInfo = application.GetServerInfo();

            Console.Title = string.Format("{0} - {1}", serverInfo.MachineName, serverInfo.IP);

            while (true)
            {
                var pushData = application.GetDataToPush();

                try
                {
                    Console.Write("pushing... ");
                    HttpPusher.Push(pushData);
                    Console.WriteLine("ok");
                }
                catch (WebException ex)
                {
                    Console.WriteLine("error");
                    Console.WriteLine(ex.Message);
                }
                catch (Exception)
                {
                    Console.WriteLine("error");
                }

                Thread.Sleep(1000);
            }
        }
    }
}
