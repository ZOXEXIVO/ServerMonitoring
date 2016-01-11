namespace ServerMonitoring.Data.Models.Common
{
    public class ServerInfo
    {
        /// <summary>
        /// Server name
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Server IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Is server active
        /// </summary>
        public bool IsActive { get; set; }

        public override int GetHashCode()
        {
            return MachineName.GetHashCode() ^ IP.GetHashCode();
        }
    }
}
