using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ServerMonitoring.Web.Application.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            Console.WriteLine(context.Exception);

            if (context.Exception is InvalidOperationException)
            {
                context.Result = new ContentResult { Content = context.Exception.ToString() };
            }
            else
            {
                context.Result = new ContentResult { Content = "SERVER ERROR" };
            }

            context.HttpContext.Response.StatusCode = 500;

            await Task.CompletedTask;
        }
    }
}
