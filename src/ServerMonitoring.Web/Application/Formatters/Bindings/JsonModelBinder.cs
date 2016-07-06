using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace ServerMonitoring.Web.Application.Formatters.Bindings
{
    public class JsonModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelStringValue = bindingContext.HttpContext.Request.Query[bindingContext.ModelName];

            var result = JsonConvert.DeserializeObject(modelStringValue, bindingContext.ModelType, JsonFormatter.SerializerSettings);

            var metadataProvider = (IModelMetadataProvider)bindingContext.HttpContext.RequestServices.GetService(typeof(IModelMetadataProvider));

            bindingContext.ModelMetadata = metadataProvider.GetMetadataForType(result.GetType());
            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}
