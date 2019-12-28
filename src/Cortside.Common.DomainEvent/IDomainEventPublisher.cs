using System;
using System.Threading.Tasks;

namespace Cortside.Common.DomainEvent {
    public delegate void PublisherClosedCallback(IDomainEventPublisher publisher, DomainEventError error);
    public interface IDomainEventPublisher {
        event PublisherClosedCallback Closed;

        Task SendAsync<T>(T @event) where T : class;
        Task SendAsync<T>(T @event, MessageOptions options) where T : class;
        Task SendAsync(string data, MessageOptions options);

        [Obsolete]
        Task SendAsync<T>(string eventType, string address, T @event) where T : class;
        [Obsolete]
        Task SendAsync<T>(string eventType, string address, T @event, string correlationId) where T : class;
        [Obsolete]
        Task SendAsync<T>(T @event, string correlationId) where T : class;
        [Obsolete]
        Task SendAsync(string eventType, string address, string data);
        [Obsolete]
        Task SendAsync(string eventType, string address, string data, string correlationId);

        Task ScheduleMessageAsync<T>(T @event, DateTime scheduledEnqueueTimeUtc) where T : class;
        Task ScheduleMessageAsync<T>(T @event, MessageOptions options, DateTime scheduledEnqueueTimeUtc) where T : class;
        Task ScheduleMessageAsync(string data, MessageOptions options, DateTime scheduledEnqueueTimeUtc);

        DomainEventError Error { get; set; }
    }
}
