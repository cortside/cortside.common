using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Services.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using Castle.Core.Logging;
using System.IO;

namespace Spring2.Common.Storage.Blob {
    public class AzureBlobProvider : IBlobProvider {

	protected String accountName;
	protected String accountKey;	
	protected String connString;

	protected CloudStorageAccount storageAccount;
	protected CloudBlobClient client;
	protected CloudBlobContainer container;
	protected ILogger log;
	protected int? maxData;
	
	public AzureBlobProvider(String protocol, String accountName, String accountKey, String containerName, ILogger logger) {
	    connString = String.Format("DefaultEndpointsProtocol={0};AccountName={1};AccountKey={2}", protocol, accountName, accountKey);
	    this.log = logger;

	    try {
		storageAccount = CloudStorageAccount.Parse(connString);
		client = storageAccount.CreateCloudBlobClient();
		container = client.GetContainerReference(containerName);
		container.CreateIfNotExists();
	    }
	    catch (Exception ex) {
		log.Error(ex.ToString());
		throw;
	    }	    
	}

	#region IBlobProvider Members

	public byte[] GetByteArray(string key) {
	    var blob = container.GetBlockBlobReference(key);
	    using(var ms = new MemoryStream()){
		blob.DownloadToStream(ms);
		return ms.ToArray();
	    }
	}

	public void Store(string key, byte[] data) {
	    var blob = container.GetBlockBlobReference(key);
	    using (var ms = new MemoryStream(data)) {
		blob.UploadFromStream(ms);
	    }
	}

	public IEnumerable<KeyValuePair<string, byte[]>> GetData() {
	    foreach(CloudBlockBlob blob in container.ListBlobs(null, true)) {
		var key = blob.Name;
		byte[] data;
		using (var ms = new MemoryStream()) {
		    blob.DownloadToStream(ms);
		    data = ms.ToArray();
		}
		var kvp = new KeyValuePair<string, byte[]>(key, data);
		yield return kvp;		
	    }
	}

	public Int32 Count() {
	    return container.ListBlobs().Count();
	}

	#endregion
    }
}
