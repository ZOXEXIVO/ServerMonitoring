using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using ServerMonitoring.WindowsAgent.Models;

namespace ServerMonitoring.WindowsAgent.Services.Transport.HttpTransport
{
    public class HttpTransportService : CoreTransportService
    {
        private string _host = "http://wsmonitoring.azurewebsites.net";

        public override async Task InternalPushDataAsync(ServerPushData data)
        {
            var request = (HttpWebRequest)WebRequest.Create(_host + "/monitoring/push");

            request.Method = "POST";
            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json";

            var jsonData = GetObjectJson(data);

            request.ContentLength = jsonData.Length;

            using (var requestStream = await request.GetRequestStreamAsync())
            {
                byte[] postBytes = Encoding.UTF8.GetBytes(jsonData);

                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();
            }

            await request.GetResponseAsync();
        }

        public override string GetInfo()
        {
            return $"transport: http\r\nhost: {_host}";
        }

        private static string GetObjectJson(ServerPushData data)
        {
            return new JavaScriptSerializer().Serialize(data);
        }
    }
}
