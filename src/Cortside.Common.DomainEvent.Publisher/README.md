## Azure Service Bus:
```
dotnet Cortside.Common.DomainEvent.Publisher.dll --key {{key}} --namespace foo.servicebus.windows.net --policy publish_user --data '{ \"fooId\":\"00000000-0000-0000-0000-000000000000\" }' --eventtype "CortSide.Common.FooStateChangedEvent" --address "foo.queue"
```

## RabbitMQ (with AMQP1.0 plugin)
```
dotnet Cortside.Common.DomainEvent.Publisher.dll --key password --namespace localhost --policy admin --data '{ \"fooId\":\"00000000-0000-0000-0000-000000000000\" }' --eventtype "Cortside.DomainEvent.Events.ELynxDocumentStateChangedEventFooStateChangedEvent" --address "document.local.queue" --protocol "amqp"
```
