using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Spring2.Common.DomainEvent.Tests {
    public class E2E : IDisposable {
	IConfigurationRoot configRoot;
	IServiceProvider serviceProvider;
	Random r;

	public E2E() {
	    r = new Random();

	    //Config
	    var config = new ConfigurationBuilder()
		.AddJsonFile("config.json");
	    configRoot = config.Build();

	    //IoC
	    var collection = new ServiceCollection();
	    collection.AddSingleton<IDomainEventHandler<TestEvent>, TestEventHandler>();
	    serviceProvider = collection.BuildServiceProvider();
	}

	public void Dispose() {
	    TestEvent.Instance = null;
	}

	[Trait("Category", "Integration")]
	[Fact]
	public async Task ShouldBeAbleToSendAndReceive() {
	    var receiverLoggerMock = new Mock<ILogger<DomainEventReceiver>>();
	    var receiverSection = configRoot.GetSection("Receiver.Settings");
	    var receiverSettings = GetSettings(receiverSection);
	    var receiver = new DomainEventReceiver(receiverSettings, serviceProvider, receiverLoggerMock.Object);

	    var publisherLoggerMock = new Mock<ILogger<DomainEventPublisher>>();
	    var publisherSection = configRoot.GetSection("Publisher.Settings");
	    var publisherSettings = GetSettings(publisherSection);
	    var publisher = new DomainEventPublisher(publisherSettings, publisherLoggerMock.Object);

	    var @event = new TestEvent {
		TheInt = r.Next(),
		TheString = Guid.NewGuid().ToString()
	    };

	    await publisher.SendAsync(@event);

	    receiver.Receive(new Dictionary<string, Type> {
		{ typeof(TestEvent).FullName, typeof(TestEvent) }
	    });
	    var start = DateTime.Now;
	    while (TestEvent.Instance == null && (DateTime.Now - start) < new TimeSpan(0, 0, 30)) {
		Thread.Sleep(1000);
	    } // run for 30 seconds
	    Assert.Equal(@event.TheString, TestEvent.Instance.TheString);
	    Assert.Equal(@event.TheInt, TestEvent.Instance.TheInt);
	}

	private ServiceBusSettings GetSettings(IConfigurationSection section) {
	    return new ServiceBusSettings {
		AppName = section["AppName"],
		Address = section["Address"],
		Key = section["Key"],
		Namespace = section["Namespace"],
		PolicyName = section["Policy"],
		Protocol = section["Protocol"]
	    };
	}
    }
}
