namespace Cortside.Common.DomainEvent {

    public enum HandlerResult {
        Success,
        Failed,
        Retry,
        Release
    };
}
