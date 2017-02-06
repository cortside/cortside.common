using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Spring2.Common.Web.Mvc {

    public class HttpExceptionMiddleware {
	private readonly RequestDelegate next;
	private readonly IOptions<MvcJsonOptions> options;

	public HttpExceptionMiddleware(RequestDelegate next, IOptions<MvcJsonOptions> options) {
	    this.next = next;
	    this.options = options;
	}

	public async Task Invoke(HttpContext context) {
	    try {
		await this.next.Invoke(context);
	    } catch (HttpException httpException) {
		context.Response.StatusCode = httpException.StatusCode;
		context.Response.ContentType = "application/json";
		/*if (httpException != null) {
		    var bytes = Encoding.UTF8.GetBytes(httpException.Message);
		    context.Response.Body = new MemoryStream(bytes);
		}*/

		string message = JsonConvert.SerializeObject(httpException, options.Value.SerializerSettings);
		await context.Response.WriteAsync(message);
	    }
	}
    }
}