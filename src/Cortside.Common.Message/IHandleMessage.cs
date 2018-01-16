namespace Cortside.Common.Message {

    public interface IHandleMessage<T> where T : IMessage {

        HandlerResultEnum Process(T message);
    }
}
