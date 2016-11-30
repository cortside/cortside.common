using Microsoft.AspNetCore.Builder;

namespace Spring2.Common.Web.Security {

    public static class ApplicationBuilderExtensions {

	public static IApplicationBuilder UseMainhostAuthentication(this IApplicationBuilder application) {
	    return application.UseMiddleware<MainhostAuthenticationMiddleware>();
	}
    }
}