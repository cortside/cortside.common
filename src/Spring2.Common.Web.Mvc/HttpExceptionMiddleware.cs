using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Spring2.Common.Web.Mvc
{

    public class HttpExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public HttpExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next.Invoke(context);
            }
            catch (HttpException httpException)
            {
                context.Response.StatusCode = httpException.StatusCode;
                context.Response.ContentType = "application/json";
                if (httpException != null)
                {
                    var bytes = Encoding.UTF8.GetBytes(httpException.Message);
                    context.Response.Body = new MemoryStream(bytes);
                }
                string message = JsonConvert.SerializeObject(httpException, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                await context.Response.WriteAsync(message);
            }
        }
    }
}