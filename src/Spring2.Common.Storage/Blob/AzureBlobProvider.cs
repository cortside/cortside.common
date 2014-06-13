using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spring2.Common.Storage.Blob {
    public class AzureBlobProvider : IBlobProvider {

	#region IBlobProvider Members

	public byte[] GetByteArray(string key) {
	    throw new NotImplementedException();
	}

	public void Store(string key, byte[] data) {
	    throw new NotImplementedException();
	}

	#endregion
    }
}
