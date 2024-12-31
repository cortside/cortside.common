[![Build status](https://ci.appveyor.com/api/projects/status/1ym8qv6j8kbkda8l?svg=true)](https://ci.appveyor.com/project/cortside/cortside-common)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=cortside_cortside.common&metric=alert_status)](https://sonarcloud.io/dashboard?id=cortside_cortside.common)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=cortside_cortside.common&metric=coverage)](https://sonarcloud.io/dashboard?id=cortside_cortside.common)

# Cortside.Common

## Cortside.Common.Threading
Threading related classes.
### Examples
```
try
{
    await DoStuffAsync().WithTimeout(TimeSpan.FromSeconds(5));
}
catch (TimeoutException)
{
    // Handle timeout.
}
```
```
var result = await DoStuffAsync().WithUnwrappedTimeout(TimeSpan.FromSeconds(5));

```
```
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
var task = Task.Run(() => DoStuffAsync()).WithCancellation(cts.Token);
```

## Cortside.Common.Messages

### Example configuration

```csharp
services.AddControllers(options => {
    options.Filters.Add<MessageExceptionResponseFilter>();
    options.Filters.Add<UnhandledExceptionFilter>();
})
.ConfigureApiBehaviorOptions(options => {
    options.InvalidModelStateResponseFactory = context => {
        var result = new ValidationFailedResult(context.ModelState);
        result.ContentTypes.Add(MediaTypeNames.Application.Json);
        return result;
    };
})
```
