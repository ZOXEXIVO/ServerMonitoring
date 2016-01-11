using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace ServerMonitoring.Web.Application.Formatters.Bindings
{
    public class JsonModelBinder : IModelBinder
    {
        //null result definition
        private static readonly ModelBindingResult NullResult = ModelBindingResult.NoResult;

        public async Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelStringValue = bindingContext.OperationBindingContext.HttpContext.Request.Query[bindingContext.ModelName];

            if(string.IsNullOrWhiteSpace(modelStringValue))
                return NullResult;

            var result = JsonConvert.DeserializeObject(modelStringValue, bindingContext.ModelType, JsonFormatter.SerializerSettings);

            return await ModelBindingResult.SuccessAsync(bindingContext.ModelName, result);
        }
    }
}
