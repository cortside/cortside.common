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

## Cortside.Common.DomainEvents
Classes for sending and listening to a message bus. Uses AMQPNETLITE (AMQP 1. 0 protocol).
### Azure ServiceBus
#### General
- Authorization keys cannot contain '/'. They must be regenerated if they do. AMQPNETLITE does not like that value.
- I found inconsistent behavior if the topic and queue were created using the AzureSB UI.  I had success creating the topics, subscriptions, queues using ServiceBusExplorer (https://github.com/paolosalvatori/ServiceBusExplorer/releases)
#### Queues
- Names of queues cannot be single worded. Should be multipart (eg. auth.queue).
#### Topic
- The forward to setting for the topic subscription is not visible in the azure UI.  You can use ServiceBusExplorer to set that field.
#### Example
- for the following configuration settings for the test project with a TestEvent object
```
    "Publisher.Settings": {
        "Protocol": "amqps",
        "Namespace": "namespace.servicebus.windows.net",
        "Policy": "Send",
        "Key": "44CharBASE64EncodedNoSlashes",
        "AppName": "test.publisher",
        "Address": "topic.",
        "Durable": "0"
    },
    "Receiver.Settings": {
        "Protocol": "amqps",
        "Namespace": "namespace.servicebus.windows.net",
        "Policy": "Listen",
        "Key": "44CharBASE64EncodedNoSlashes",
        "AppName": "test.receiver",
        "Address": "queue.testReceive",
        "Durable": "0"
    }
```
**__(for test default settings from Service Bus Explorer are fine unless specified below)__**
- Azure Service Bus Components:
  - a queue named queue.TestReceive
    - new authorization rule for queue
      - claimType = SharedAccessKey
      - claimValue = none
      - KeyName = "Listen"
      - Primary/Secondary Key = 44 Char BASE64 encoded string (33 char unencoded and remember no '/')
      - Manage - off
      - Send - off
      - Listen - on
  - a topic named topic.TestEvent
    - new authorization rule for topic
      - claimType = SharedAccessKey
      - claimValue = none
      - KeyName = "Send"
      - Primary/Secondary Key = 44 Char BASE64 encoded string (33 char unencoded and remember no '/')
      - Manage - off
      - Send - on
      - Listen - off
  - a subscription to topic.TestEvent named subscription.TestEvent
    - The "Forward To" setting for this subscription needs to be set to queue.TestReceive