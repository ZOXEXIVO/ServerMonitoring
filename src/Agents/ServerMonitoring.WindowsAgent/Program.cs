using System;
using ServerMonitoring.WindowsAgent.Application;

namespace ServerMonitoring.WindowsAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            string host;

            if (args.Length == 0)
            {
                Console.WriteLine("monitoring host: ");
                host = Console.ReadLine();
            }
            else
            {
                host = args[0];
            }

            if (string.IsNullOrWhiteSpace(host))
            {
                Console.WriteLine("host is missing");
                Console.Read();

                return;
            }

            if (!host.StartsWith("http://"))
                host = "http://" + host;

            new MonitoringApp(host).Start();
        }
    }
}
