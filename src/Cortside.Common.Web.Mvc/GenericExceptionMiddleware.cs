using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Cortside.Common.Web.Mvc {
    public class GenericExceptionMiddleware {
        private readonly RequestDelegate next;
        private readonly IOptions<MvcJsonOptions> options;

        public GenericExceptionMiddleware(RequestDelegate next, IOptions<MvcJsonOptions> options) {
            this.next = next;
            this.options = options;
        }

        public async Task Invoke(HttpContext context) {
            try {
                await this.next.Invoke(context);
            } catch (Exception exception) {

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";


                string message = JsonConvert.SerializeObject(exception, options.Value.SerializerSettings);
                await context.Response.WriteAsync(message);
            }
        }
    }
}
