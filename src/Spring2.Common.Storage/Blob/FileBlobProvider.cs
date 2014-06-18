using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spring2.Common.Storage.Blob {
    public class FileBlobProvider : IBlobProvider {

	public String RootFolder { get; private set; }

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

	public IEnumerable<KeyValuePair<string, byte[]>> GetData() {
	    return (from file in Directory.EnumerateFiles(RootFolder, "*", SearchOption.AllDirectories)
		    select new KeyValuePair<string, byte[]>(file, File.ReadAllBytes(file))
		    );
	}

	public Int32 Count() {
	    return (from file in Directory.EnumerateFiles(RootFolder, "*", SearchOption.AllDirectories)
		    select file
		    ).Count();
	}

	#endregion

	protected String GetPath(string blobKey) {
	    return String.Format("{0}/{1}", RootFolder, blobKey);
	}
    }
}
