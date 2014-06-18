using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spring2.Common.Storage.Blob {
    public interface IBlobProvider {
	byte[] GetByteArray(string key);
	void Store(string key, byte[] data);
	IEnumerable<KeyValuePair<string, byte[]>> GetData();
	int Count();
    }
}
