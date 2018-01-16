using System;

namespace Cortside.Common.Web.Security {

    public class TokenPayload {
        public int iat { get; set; }

        public int exp { get; set; }
        public string iss { get; set; }
        public String sub { get; set; }
    }
}
