namespace ServerMonitoring.WindowsAgent.Services.Output
{
    public interface IOutputService
    {
        void Info(string message);
        void InfoLine(string message);
    }
}
