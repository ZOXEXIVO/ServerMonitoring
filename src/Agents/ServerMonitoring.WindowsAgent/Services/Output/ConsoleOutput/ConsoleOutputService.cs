using System;

namespace ServerMonitoring.WindowsAgent.Services.Output.ConsoleOutput
{
    public class ConsoleOutputService : IOutputService
    {
        public void Info(string message)
        {
            Console.Write(message);
        }

        public void InfoLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
