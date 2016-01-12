using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using ServerMonitoring.WindowsAgent.Models;

namespace ServerMonitoring.WindowsAgent.Pusher
{
    public class HttpPusher
    {
        public void Push(ServerPushData data)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("http://localhost:5000/monitoring/push");

                request.Method = "POST";
                request.ContentType = "application/json; charset=UTF-8";
                request.Accept = "application/json";

                var jsonData = GetObjectJson(data);

                request.ContentLength = jsonData.Length;

                Stream requestStream = request.GetRequestStream();

                byte[] postBytes = Encoding.UTF8.GetBytes(jsonData);

                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Console.WriteLine("ok");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private string GetObjectJson(ServerPushData data)
        {
            return new JavaScriptSerializer().Serialize(data);
        }
    }
}
