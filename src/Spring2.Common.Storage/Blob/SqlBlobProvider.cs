using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;

namespace Spring2.Common.Storage.Blob {
    public class SqlBlobProvider : IBlobProvider {

	public ILogger Log { get; private set; }
	public string ConnectionString { get; private set; }
	public string BlobTable { get; private set; }
	public string KeyColumn { get; private set; }
	public string DataColumn { get; private set; }

	public SqlBlobProvider(String connectionString, string blobTable, string keyColumn, string dataColumn, ILogger logger) {
	    this.ConnectionString = connectionString;
	    this.BlobTable = blobTable;
	    this.KeyColumn = keyColumn;
	    this.DataColumn = dataColumn;
	    this.Log = logger;
	}

	#region IBlobProvider Members

	public byte[] GetByteArray(string key) {
	    if (String.IsNullOrEmpty(key)) {
		return null;
	    }

	    if (Log.IsDebugEnabled) {
		Log.DebugFormat("Getting data with key {0}...", key);
	    }

	    using (var conn = new SqlConnection(ConnectionString)) {
		var qry = String.Format("select [{0}] from [{1}] where [{2}] = @key", DataColumn, BlobTable, KeyColumn);

		using (var cmd = new SqlCommand(qry, conn)) {
		    cmd.Parameters.AddWithValue("key", key);
		    conn.Open();
		    var obj = cmd.ExecuteScalar();
		    if (obj != null && obj != DBNull.Value) {
			return (byte[])obj;
		    }
		}
		return null;
	    }
	}

	public void Store(string key, byte[] data) {
	    if (String.IsNullOrEmpty(key) || data == null) {
		if (Log.IsInfoEnabled) {
		    Log.InfoFormat("Not storing {0} because data is empty.", key);
		}
		return;
	    }

	    if (Log.IsDebugEnabled) {
		Log.DebugFormat("Storing {0}...", key);
	    }

	    using (var conn = new SqlConnection(ConnectionString)) {
		var qry = String.Format("if not exists (select 1 from [{0}] where [{1}] = @key) insert into [{0}] ([{1}], [{2}]) values (@key, @data) else update [{0}] set [{2}] = @data where [{1}] = @key",
		    BlobTable, KeyColumn, DataColumn
		);

		using (var cmd = new SqlCommand(qry, conn)) {
		    cmd.Parameters.AddWithValue("key", key);
		    cmd.Parameters.AddWithValue("data", data);
		    conn.Open();
		    cmd.ExecuteNonQuery();
		}
	    }
	}

	public IEnumerable<KeyValuePair<string, byte[]>> GetData() {
	    var conn = new SqlConnection(ConnectionString);
	    var qry = String.Format("select [{0}], [{1}] from [{2}] where [{0}] is not null and [{1}] is not null order by [{0}]", KeyColumn, DataColumn, BlobTable);
	    var cmd = new SqlCommand(qry, conn);
	    conn.Open();
	    var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
	    while (dr.Read()) {
		string key = dr.GetString(0);
		byte[] data = (byte[])dr[1];
		var kvp = new KeyValuePair<string, byte[]>(key, data);
		yield return kvp;
	    }
	    dr.Close();
	}

	public int Count() {
	    using (var conn = new SqlConnection(ConnectionString)) {
		var qry = String.Format("select count(*) from [{0}]", BlobTable);
		using (var cmd = new SqlCommand(qry, conn)) {
		    conn.Open();
		    var obj = cmd.ExecuteScalar();
		    return (int)obj;
		}
	    }
	}

	#endregion
    }
}
