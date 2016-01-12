using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerMonitoring.Web.Application.Filters;
using ServerMonitoring.Web.Application.Formatters;

namespace ServerMonitoring.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }
     
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                var jsonFormatter = new JsonFormatter();

                options.InputFormatters.Clear();
                options.InputFormatters.RemoveType<JsonInputFormatter>();
                options.InputFormatters.Add(jsonFormatter);

                options.OutputFormatters.Clear();
                options.OutputFormatters.RemoveType<JsonOutputFormatter>();
                options.OutputFormatters.Add(jsonFormatter);

                options.Filters.Add(new ExceptionFilter());
            });

            services.RegisterInternalServices();
        }
     
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseCors();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Startup}/{action=Index}/{id?}");
            });
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
