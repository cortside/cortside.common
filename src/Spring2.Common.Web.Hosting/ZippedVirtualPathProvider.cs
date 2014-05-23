using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Spring2.Common.Web.Hosting {
    /// <summary>
    /// A virtual path provider that extracts content from .zip files
    /// </summary>
    public class ZippedVirtualPathProvider : VirtualPathProvider {
	private bool mappedServerPath;
	private string zipFilename;

	/// <summary>
	/// Register this class as the new virtual path provider
	/// </summary>
	public static void RegisterMe(String zipFilename = "~/Resources.zip", Boolean mappedServerPath = true) {
	    HostingEnvironment.RegisterVirtualPathProvider(new ZippedVirtualPathProvider(zipFilename, mappedServerPath));
	}

	public ZippedVirtualPathProvider(String zipFilename = "~/Resources.zip", Boolean mappedServerPath = true) {
	    this.zipFilename = zipFilename;
	    this.mappedServerPath = mappedServerPath;
	}

	#region VirtualPathProvider overrides

	/// <summary>
	/// Overridden method. Check if a given file exists.
	/// </summary>
	/// <param name="virtualPath"></param>
	/// <returns></returns>
	public override bool FileExists(string virtualPath) {
	    // if zip is existing and the file exists in the zip, we can return true
	    if (IsViewsZipFound && FileExistsInZip(virtualPath)) {
		return true;
	    }
		// otherwise let the base implementation handle it
	    else {
		return base.FileExists(virtualPath);
	    }
	}

	/// <summary>
	/// Get a file
	/// </summary>
	/// <param name="virtualPath"></param>
	/// <returns></returns>
	public override VirtualFile GetFile(string virtualPath) {
	    if (IsViewsZipFound) {
		var file = ExtractFromZip(virtualPath);
		if (file != null) {
		    return file;
		}
	    }

	    return base.GetFile(virtualPath);
	}

	/// <summary>
	/// Get a cache dependency for a given virtual path
	/// </summary>
	/// <param name="virtualPath"></param>
	/// <param name="virtualPathDependencies"></param>
	/// <param name="utcStart"></param>
	/// <returns></returns>
	public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart) {
	    // if the file is handled by us, return a dependency for the .zip file
	    if (IsViewsZipFound && FileExistsInZip(virtualPath)) {
		return new CacheDependency(ViewsZipPath, utcStart);
	    } else {
		return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
	    }
	}

	#endregion

	#region Zip management

	/// <summary>
	/// Check if a file exists in a zip
	/// </summary>
	/// <param name="virtualPath"></param>
	/// <returns></returns>
	private bool FileExistsInZip(string virtualPath) {
	    string zipPath = TransformVirtualPath2ZipPath(virtualPath);
	    using (ZipArchive archive = new ZipArchive(File.OpenRead(ViewsZipPath))) {
		return archive.Entries.Any(x => x.FullName.Equals(zipPath, StringComparison.CurrentCultureIgnoreCase));
	    }
	}

	/// <summary>
	/// Extract a file from a zip
	/// </summary>
	/// <param name="virtualPath"></param>
	/// <returns></returns>
	private VirtualFile ExtractFromZip(string virtualPath) {
	    string zipPath = TransformVirtualPath2ZipPath(virtualPath);

	    using (ZipArchive archive = new ZipArchive(File.OpenRead(ViewsZipPath))) {
		ZipArchiveEntry entry = archive.Entries.FirstOrDefault(x => x.FullName.Equals(zipPath, StringComparison.CurrentCultureIgnoreCase));
		if (entry != null) {
		    using (Stream stream = entry.Open()) {
			byte[] data = new byte[entry.Length];
			stream.Read(data, 0, data.Length);
			return new ZippedVirtualFile(virtualPath, data);
		    }
		}
		return null;
	    }
	}

	#endregion

	#region Utility methods

	/// <summary>
	/// If the Views.zip file found in the file system
	/// </summary>
	private bool IsViewsZipFound {
	    get {
		return File.Exists(ViewsZipPath);
	    }
	}

	/// <summary>
	/// What is the physical path for the Views.zip file
	/// </summary>
	private string ViewsZipPath {
	    get {
		if (mappedServerPath) {
		    return HttpContext.Current.Server.MapPath(zipFilename);
		} else {
		    return zipFilename;
		}
	    }
	}

	/// <summary> 
	/// Transform a virtual path into a file used inside the .zip 
	/// </summary> 
	/// <param name="virtualPath"></param> 
	/// <returns></returns> 
	/// <remarks>Just remove some slashes from the front</remarks> 
	private static string TransformVirtualPath2ZipPath(string virtualPath) {
	    string fullPath = GetVirtualPath(virtualPath);

	    if (fullPath.StartsWith("~/")) {
		fullPath = fullPath.Substring(2);
	    }
	    if (fullPath.StartsWith("/")) {
		fullPath = fullPath.Substring(1);
	    }
	    return fullPath;
	}

	private static string GetVirtualPath(string url) {
	    string appPath = HttpContext.Current.Request.ApplicationPath;
	    //If you can write this better while still keeping the same string manipulation, please do!
	    if (appPath == "/") {
		if (url.StartsWith("~/")) {
		    return url;
		} else if (url.StartsWith("/")) {
		    return "~" + url;
		}
		return "~/" + url;
	    } else {
		if (!url.StartsWith("/") && !url.StartsWith("~/")) {
		    url = "/" + url;
		}
		if (url.StartsWith("/")) {
		    url = "~" + url;
		}
		return Regex.Replace(url, "^~" + appPath + "(.+)$", "~$1");
	    }
	}

	#endregion
    }
}
