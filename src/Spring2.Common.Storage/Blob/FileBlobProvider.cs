using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spring2.Common.Storage.Blob {
    public class FileBlobProvider : IBlobProvider {

	public String RootFolder { get; set; }

	public FileBlobProvider(string rootFolder) {
	    this.RootFolder = rootFolder;
	}

	#region IBlobProvider Members

	public byte[] GetByteArray(string key) {
	    var path = GetPath(key);
	    return File.ReadAllBytes(path);
	}

	public void Store(string key, byte[] data) {
	    var path = GetPath(key);
	    var dir = Path.GetDirectoryName(path);
	    Directory.CreateDirectory(dir);
	    File.WriteAllBytes(path, data);
	}

	protected String GetPath(string blobKey) {
	    return String.Format("{0}/{1}", RootFolder, blobKey);
	}

	#endregion
    }
}
