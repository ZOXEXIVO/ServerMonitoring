using System;

namespace ServerMonitoring.WindowsAgent.SizeHelper
{
    public static class SizeHelper
    {
        public static Tuple<long, string> GetSizeDisplay(long bytes)
        {
            if (bytes < 1024)
            {
                return new Tuple<long, string>(bytes, "B");
            }

            if (bytes < 1024 * 1024)
            {
                return new Tuple<long, string>(bytes / 1024, "KB");
            }

            if (bytes < 1024 * 1024 * 1024)
            {
                return new Tuple<long, string>(bytes / (1024 * 1024), "MB");
            }

            return new Tuple<long, string>(bytes / (1024 * 1024 * 1024), "GB");
        }
    }
}
