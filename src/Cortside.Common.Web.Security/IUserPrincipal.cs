using System;
using System.Security.Principal;

namespace Cortside.Common.Web.Security {

    public interface IUserPrincipal : IPrincipal {

        Int32 UserId {
            get;
        }
    }
}
