using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using log4net.Config;

namespace Spring2.Common.Storage.Blob.ExportUtil {
    public class ServiceProcess {

	private static log4net.ILog log;
	private static IExportBlobs svc;
	private static IWindsorContainer container;	
	private static Mutex mutex;

	public static void Main(string[] args) {
	    InitLogger();
	    log.Info("Starting...");

	    try {
		bool ok = false;
		string appID = ((GuidAttribute)GuidAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(GuidAttribute))).Value;
		using (mutex = new Mutex(true, appID, out ok)) {
		    if (!ok) {
			Environment.Exit(0);
		    }

		    InitContainer();
		    
		    IBlobProvider src = container.Resolve<IBlobProvider>("sourceProvider");
		    IBlobProvider dest = container.Resolve<IBlobProvider>("destinationProvider");
		    svc = container.Resolve<IExportBlobs>(new { sourceProvider = src, destinationProvider = dest });
		    
		    svc.Run();		    
		}
	    }
	    catch (Exception ex) {
		log.Fatal(ex.Message);
	    }
	    log.Info("Finished.\n");
	    Console.ReadKey();
	}

	protected static void InitLogger() {
	    log4net.MDC.Set("hostname", Environment.MachineName);
	    log = log4net.LogManager.GetLogger(typeof(ServiceProcess));
	    BasicConfigurator.Configure();
	}

	protected static void InitContainer() {
	    container = new WindsorContainer();
	    container.AddFacility<LoggingFacility>(f => f.UseLog4Net());
	    container.Install(Configuration.FromAppConfig());
	    container.AddFacility<TypedFactoryFacility>();
	    container.Register(Component.For<IExportBlobs>().ImplementedBy<ExportBlobs>().LifeStyle.Transient);
	}
	
    }
}
