using System;
using System.Security.Principal;
using System.Threading;

namespace Spring2.Common.Web.Security {

    public class UserPrincipal : IUserPrincipal {
	private Int32 userId;
	private WindowsPrincipal windowsPrincipal;

	public UserPrincipal() {
	}

	public UserPrincipal(Int32 userId) {
	    this.userId = userId;
	    windowsPrincipal = Thread.CurrentPrincipal as WindowsPrincipal;
	}

	public Int32 UserId {
	    get {
		return userId;
	    }
	}

	public IIdentity Identity {
	    get {
		return windowsPrincipal.Identity;
	    }
	}

	public bool IsInRole(string role) {
	    throw new System.NotImplementedException();
	}
    }
}