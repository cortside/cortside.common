using System;
using System.Threading.Tasks;

namespace Cortside.Common.DomainEvent {

    public delegate void PublisherClosedCallback(IDomainEventPublisher publisher, DomainEventError error);

    public interface IDomainEventPublisher {
        event PublisherClosedCallback Closed;

        Task SendAsync<T>(T @event) where T : class;
        Task SendAsync<T>(T @event, string correlationId) where T : class;
        Task SendAsync<T>(T @event, string correlationId, string messageId) where T : class;
        Task SendAsync<T>(T @event, string eventType, string address, string correlationId) where T : class;

        Task SendAsync(string eventType, string address, string data, string correlationId, string messageId);

        Task ScheduleMessageAsync<T>(T @event, DateTime scheduledEnqueueTimeUtc) where T : class;
        Task ScheduleMessageAsync<T>(T @event, string correlationId, DateTime scheduledEnqueueTimeUtc) where T : class;
        Task ScheduleMessageAsync<T>(T @event, string correlationId, string messageId, DateTime scheduledEnqueueTimeUtc) where T : class;

        Task ScheduleMessageAsync<T>(T @event, string eventType, string address, string correlationId, DateTime scheduledEnqueueTimeUtc) where T : class;

        Task ScheduleMessageAsync(string data, string eventType, string address, string correlationId, string messageId, DateTime scheduledEnqueueTimeUtc);

        DomainEventError Error { get; set; }
    }
}
