using Microsoft.AspNetCore.Builder;

namespace Spring2.Common.Web.Mvc {

    public static class ApplicationBuilderExtensions {

	public static IApplicationBuilder UseHttpException(this IApplicationBuilder application) {
	    return application.UseMiddleware<HttpExceptionMiddleware>();
	}
    }
}