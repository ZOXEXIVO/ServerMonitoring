namespace ServerMonitoring.Data.Models.Common
{
    public class ServerInfo
    {
        /// <summary>
        /// Server ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Server name
        /// </summary>
        public string MachineName { get; set; }
        
        /// <summary>
        /// Is server active
        /// </summary>
        public bool IsActive { get; set; }
    }
}
