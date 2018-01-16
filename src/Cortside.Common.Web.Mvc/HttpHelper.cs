using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Cortside.Common.Web.Mvc {

    public static class HttpHelper {
        private static IHttpContextAccessor HttpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor) {
            HttpContextAccessor = httpContextAccessor;
        }

        public static HttpContext HttpContext {
            get {
                return HttpContextAccessor?.HttpContext;
            }
        }

        public static bool IsPreflight() {
            return IsPreflight(HttpHelper.HttpContext);
        }

        public static bool IsPreflight(HttpContext context) {
            return context.Request.Headers.Any(k => k.Key.Contains("Origin")) && context.Request.Method == "OPTIONS";
        }
    }
}
