using System.Collections.Generic;

namespace ServerMonitoring.WindowsAgent.Services.CPU
{
    public interface ICpuNameService
    {
        IEnumerable<string> GetCPUNames();
    }
}
