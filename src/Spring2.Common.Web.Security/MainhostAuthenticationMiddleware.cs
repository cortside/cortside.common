using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spring2.Common.Web.Security {

    public class MainhostAuthenticationMiddleware {
	private readonly RequestDelegate next;

	public MainhostAuthenticationMiddleware(RequestDelegate next) {
	    this.next = next;
	}

	public async Task Invoke(HttpContext context) {
	    // http://odetocode.com/blogs/scott/archive/2015/01/15/using-json-web-tokens-with-katana-and-webapi.aspx

	    try {
		var user = Authorization.Authorize(context);

		var identity = new ClaimsIdentity();
		identity.AddClaim(new Claim("sub", user.UserId.ToString()));
		context.User = new ClaimsPrincipal(identity);
	    } catch (Exception) {
		// catch and swallow any problems, no principal will be set
	    }

	    await next.Invoke(context);
	}
    }
}