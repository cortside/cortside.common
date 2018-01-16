using System;
using Cortside.Common.Web.Mvc;
using Microsoft.AspNetCore.Http;

namespace Cortside.Common.Web.Security {

    public class Authorization {

        // TODO: config file
        private const string JWT_SECRET = "47GbL1!b2#68a23R4x9174awfk7de390&f";

        public static UserPrincipal Authorize() {
            return Authorize(HttpHelper.HttpContext);
        }

        public static UserPrincipal Authorize(HttpContext context) {
            // TODO: check exists?
            var header = context.Request.Headers["Authorization"];

            if (header.Count == 0) {
                throw new Exception("missing Authorization header -- 401");
            }

            // TODO: got something?
            if (!header[0].StartsWith("Bearer ")) {
                throw new Exception("invalid authorization header");
            }
            string token = header[0].Substring(7);

            // TODO: This method of validating the payload by decoding it using a statc JWT_SECRET is outdated and vulnerable
            TokenPayload payload = JWT.DecodeToObject<TokenPayload>(token, JWT_SECRET, true);

            // TODO: set principal on thread

            return new UserPrincipal(Int32.Parse(payload.sub));
        }
    }
}
