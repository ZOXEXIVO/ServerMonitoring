using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using ServerMonitoring.Data;

namespace ServerMonitoring.Web
{
    public static class ServiceRegistration
    {
        public static void RegisterInternalServices(this IServiceCollection services)
        {
            services.RegisterDataServices();
        }
       
        public static void UseCors(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Method != "OPTIONS")
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                    await next();

                    return;
                }

                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "origin, content-type, accept, token");
                context.Response.Headers.Add("Access-Control-Max-Age", "1728000");
            });
        }
    }
}
