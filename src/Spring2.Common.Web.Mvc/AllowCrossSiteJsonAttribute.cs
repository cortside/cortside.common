using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;

namespace Spring2.Common.Web.Mvc {

    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute {

	public override void OnActionExecuting(ActionExecutingContext actionExecutingContext) {
	    HttpHelper.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
	    HttpHelper.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization, X-Requested-With, Content-Type");
	    HttpHelper.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
	    HttpHelper.HttpContext.Response.Headers.Add("Access-Control-Max-Age", "900");

	    if (HttpHelper.IsPreflight(HttpHelper.HttpContext)) {
		actionExecutingContext.Result = new EmptyResult();
	    } else {
		base.OnActionExecuting(actionExecutingContext);
	    }
	}
    }
}