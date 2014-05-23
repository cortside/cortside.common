using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Spring2.Common.Web.Hosting {
    /// <summary>
    /// A class to store a single file that was extracted from a .zip archive
    /// </summary>
    public class ZippedVirtualFile : VirtualFile {
	/// <summary>
	/// The actual file data. Uncompressed
	/// </summary>
	public byte[] data;

	/// <summary>
	/// Initialize a new instance of the class
	/// </summary>
	/// <param name="virtualPath"></param>
	/// <param name="data"></param>
	public ZippedVirtualFile(string virtualPath, byte[] data)
	    : base(virtualPath) {
	    this.data = data;
	}

	/// <summary>
	/// Return a new stream to the uncompressed bytes of the file
	/// </summary>
	/// <returns></returns>
	public override Stream Open() {
	    return new MemoryStream(data);
	}
    }
}
