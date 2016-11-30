using System;
using Microsoft.Extensions.Configuration;

namespace Spring2.Common.IoC {
    public static class DI {
	private static IServiceProvider privateContainer;
	private static object lockObject;
	private static IConfigurationRoot configuration;

	static DI() {
	    lockObject = new object();
	}

	public static void SetContainer(IServiceProvider container) {
	    lock (lockObject) {
		privateContainer = container;
	    }
	}

	public static IServiceProvider Container {
	    get { return privateContainer; }
	}

	public static void SetConfiguration(IConfigurationRoot configurationRoot) {
	    lock (lockObject) {
		configuration = configurationRoot;
	    }
	}

	public static IConfigurationRoot Configuration {
	    get { return configuration; }
	}
    }
}