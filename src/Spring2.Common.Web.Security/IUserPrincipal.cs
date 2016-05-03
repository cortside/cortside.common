using System;
using System.Security.Principal;

namespace Spring2.Common.Web.Security {

    public interface IUserPrincipal : IPrincipal {

	Int32 UserId {
	    get;
	}
    }
}