using System;
using System.Threading.Tasks;

namespace Cortside.Common.DomainEvent {
    public delegate void PublisherClosedCallback(IDomainEventPublisher publisher, DomainEventError error);
    public interface IDomainEventPublisher {
        event PublisherClosedCallback Closed;
        Task SendAsync<T>(string eventType, string address, T @event) where T : class;
        Task SendAsync<T>(string eventType, string address, T @event, string correlationId) where T : class;
        Task SendAsync<T>(T @event) where T : class;
        Task SendAsync<T>(T @event, string correlationId) where T : class;
        Task SendAsync(string eventType, string address, string data);
        Task SendAsync(string eventType, string address, string data, string correlationId);

        Task ScheduleMessageAsync<T>(T @event, DateTime scheduledEnqueueTimeUtc) where T : class;
        Task ScheduleMessageAsync<T>(T @event, string correlationId, DateTime scheduledEnqueueTimeUtc) where T : class;
        Task ScheduleMessageAsync<T>(string eventType, string address, T @event, DateTime scheduledEnqueueTimeUtc) where T : class;
        Task ScheduleMessageAsync<T>(string eventType, string address, T @event, string correlationId, DateTime scheduledEnqueueTimeUtc) where T : class;
        Task ScheduleMessageAsync(string eventType, string address, string data, DateTime scheduledEnqueueTimeUtc);
        Task ScheduleMessageAsync(string eventType, string address, string data, string correlationId, DateTime scheduledEnqueueTimeUtc);

        DomainEventError Error { get; set; }
    }
}
