using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Spring2.Common.Web.Mvc
{
    public class GenericExceptionMiddleware
    {
        private readonly RequestDelegate next;

        // example: https://github.com/aspnet/Diagnostics/blob/dev/src/Microsoft.AspNetCore.Diagnostics/DeveloperExceptionPage/DeveloperExceptionPageMiddleware.cs#L69-L106
        public GenericExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next.Invoke(context);
            }
            catch (Exception exception)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                string message = JsonConvert.SerializeObject(exception, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                });
                await context.Response.WriteAsync(message);
            }
        }
    }
}
