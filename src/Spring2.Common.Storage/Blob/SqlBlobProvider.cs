using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spring2.Common.Storage.Blob {
    public class SqlBlobProvider : IBlobProvider {

	public string ConnectionString { get; private set; }
	public string BlobTable { get; private set; }
	public string KeyColumn { get; private set; }
	public string DataColumn { get; private set; }

	public SqlBlobProvider(String connectionString, string blobTable, string keyColumn, string dataColumn) {
	    this.ConnectionString = connectionString;
	    this.BlobTable = blobTable;
	    this.KeyColumn = keyColumn;
	    this.DataColumn = dataColumn;
	}

	#region IBlobProvider Members

	public byte[] GetByteArray(string key) {
	    using (var conn = new SqlConnection(ConnectionString)) {
		var qry = String.Format("select [{0}] from [{1}] where [{2}] = @key", DataColumn, BlobTable, KeyColumn);
		var cmd = new SqlCommand(qry, conn);
		cmd.Parameters.AddWithValue("key", key);		
		conn.Open();
		var obj = cmd.ExecuteScalar();
		if (obj != null) {
		    return (byte[])obj;
		}
		return null;
	    }
	}

	public void Store(string key, byte[] data) {
	    using (var conn = new SqlConnection(ConnectionString)) {
		var qry = String.Format("if not exists (select 1 from [{0}] where [{1}] = @key) insert into [{0}] ([{1}], [{2}]) values (@key, @data) else update [{0}] set [{2}] = @data where [{1}] = @key",
		    BlobTable, KeyColumn, DataColumn
		);
		
		var cmd = new SqlCommand(qry, conn);
		cmd.Parameters.AddWithValue("key", key);
		cmd.Parameters.AddWithValue("data", data);
		conn.Open();
		cmd.ExecuteNonQuery();
	    }
	}

	#endregion
    }
}
