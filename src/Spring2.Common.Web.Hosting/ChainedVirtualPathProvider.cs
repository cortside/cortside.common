using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Hosting;

/*
Using the ChainedVirtualPathProvider 
As a final step, we need to register our provider. This can be done in global.asax.cs by adding the following code to the Application_Start() method: 

protected void Application_Start() {
    VirtualPathProvider primary = new ....;
    VirtualPathProvider fallback = new ....;
  
    ChainedVirtualPathProvider.RegisterMe(primary, fallback);
}

I've added the following to my web.config inside the <system.webServer> tags and now the image and css files are calling the methods in my code.
 
<handlers>
  <add name="Images" path="*.png" verb="GET,HEAD,POST" type="System.Web.StaticFileHandler" modules="ManagedPipelineHandler" resourceType="Unspecified" />
  <add name="Stylesheets" path="*.css" verb="GET,HEAD,POST" type="System.Web.StaticFileHandler" modules="ManagedPipelineHandler" resourceType="Unspecified" />
</handlers>

 */

namespace Spring2.Common.Web.Hosting {
    /// <summary>
    /// A virtual path provider that extracts content from .zip files
    /// </summary>
    public class ChainedVirtualPathProvider : VirtualPathProvider {
	private VirtualPathProvider[] providers;

	/// <summary>
	/// Register this class as the new virtual path provider
	/// </summary>
	public static void RegisterMe(params VirtualPathProvider[] providers) {
	    HostingEnvironment.RegisterVirtualPathProvider(new ChainedVirtualPathProvider(providers));
	}

	public ChainedVirtualPathProvider(params VirtualPathProvider[] providers) {
	    if (providers.Length == 0 && HostingEnvironment.VirtualPathProvider != null) {
		providers = new VirtualPathProvider[] { HostingEnvironment.VirtualPathProvider };
	    }
	    this.providers = providers;
	}

	#region VirtualPathProvider overrides

	/// <summary>
	/// Overridden method. Check if a given file exists.
	/// </summary>
	/// <param name="virtualPath"></param>
	/// <returns></returns>
	public override bool FileExists(string virtualPath) {
	    foreach (VirtualPathProvider provider in providers) {
		if (provider.FileExists(virtualPath)) {
		    return true;
		}
	    }
	    return false;
	}

	/// <summary>
	/// Get a file
	/// </summary>
	/// <param name="virtualPath"></param>
	/// <returns></returns>
	public override VirtualFile GetFile(string virtualPath) {
	    foreach (VirtualPathProvider provider in providers) {
		if (provider.FileExists(virtualPath)) {
		    return provider.GetFile(virtualPath);
		}
	    }
	    return null;
	}

	/// <summary>
	/// Get a cache dependency for a given virtual path
	/// </summary>
	/// <param name="virtualPath"></param>
	/// <param name="virtualPathDependencies"></param>
	/// <param name="utcStart"></param>
	/// <returns></returns>
	public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart) {
	    foreach (VirtualPathProvider provider in providers) {
		if (provider.FileExists(virtualPath)) {
		    return provider.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
		}
	    }
	    return null;
	}

	#endregion
    }
}
