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

