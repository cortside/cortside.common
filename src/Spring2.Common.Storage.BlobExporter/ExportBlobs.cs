using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;

namespace Spring2.Common.Storage.Blob.ExportUtil {
    public class ExportBlobs : IExportBlobs {

	protected IBlobProvider src;
	protected IBlobProvider dst;
	protected ILogger log;

	public ExportBlobs(IBlobProvider sourceProvider, IBlobProvider destinationProvider, ILogger logger) {
	    this.src = sourceProvider;
	    this.dst = destinationProvider;
	    this.log = logger;
	}

	#region IExportBlobs Members

	public void Run() {
	    Stopwatch st = new Stopwatch();
	    st.Start();
	    double i = 0;
	    double count = src.Count();

	    foreach (var kvp in src.GetData()) {
		i++;
		dst.Store(kvp.Key, kvp.Value);
		int percentComplete = (int)((i / count) * 100);
		Console.Write("\rStoring {0} of {1} blobs. {2}% Complete...", i, count, percentComplete);
	    }

	    st.Stop();
	    log.InfoFormat("Total Time for blob export: {0} ms.", st.ElapsedMilliseconds);
	    log.Info("Done.");
	}

	#endregion
    }
}
