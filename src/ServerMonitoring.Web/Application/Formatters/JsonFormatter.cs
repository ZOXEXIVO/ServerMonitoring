using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ServerMonitoring.Web.Application.Formatters
{
    public class JsonFormatter : IOutputFormatter, IInputFormatter
    {
        public static readonly JsonSerializerSettings SerializerSettings;

        static JsonFormatter()
        {
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public bool CanRead(InputFormatterContext context)
        {
            return true;
        }

        public async Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            using (var requestStream = new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8))
            {
                var requestData = await requestStream.ReadToEndAsync();
                return InputFormatterResult.Success(JsonConvert.DeserializeObject(requestData, context.ModelType, SerializerSettings));
            }
        }
        
        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return true;
        }

        public async Task WriteAsync(OutputFormatterWriteContext context)
        {
            using (var responceStream = new StreamWriter(context.HttpContext.Response.Body, Encoding.UTF8))
            {
                await responceStream.WriteAsync(JsonConvert.SerializeObject(context.Object, SerializerSettings));
            }
        }
    }
}
