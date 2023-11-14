# Cortside.Common.Testing

* Update nuget dependencies to latest stable versions
* Add ScopedLocalTimeZone for use with tests to set predictable timezone regardless of environment
    ```csharp
    using (new ScopedLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("UTC+12"))) {
        var localDateTime = new DateTime(2020, 12, 31, 23, 59, 59, DateTimeKind.Local);
        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime);

        Assert.Equal("UTC+12", TimeZoneInfo.Local.Id);
        Assert.Equal(utcDateTime.AddHours(12), localDateTime);
    }
    ```
* Add XunitLogger for capturing log output to xUnit's ITestOutputHelper
	```csharp
	// Create a logger factory with a debug provider
	loggerFactory = LoggerFactory.Create(builder => {
		builder
			.SetMinimumLevel(LogLevel.Trace)
			.AddFilter("Microsoft", LogLevel.Warning)
			.AddFilter("System", LogLevel.Warning)
			.AddFilter("Cortside.Common", LogLevel.Trace)
			.AddXunit(output);
	});

	// Create a logger with the category name of the current class
	var logger = loggerFactory.CreateLogger<XunitLoggerTest>();

	... user logger as you would any other logger ...
	```
* changed hierarchichal organization of loggers, so LogEventLogger has new namespace of Cortside.Common.Testing.Logging.LogEvent
* Added helper class RandomValues for generating "random" data
* Added IServiceCollection extension method Unregister<T> for unregistering something already in service collection
	```csharp
	// Remove the app's DbContext registration.
	services.Unregister<DbContextOptions<DatabaseContext>>();
	services.Unregister<DbContext>();
	```
