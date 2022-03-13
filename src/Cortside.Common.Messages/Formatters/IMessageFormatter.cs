namespace Cortside.Common.Messages.Formatters {
    public interface IMessageFormatter {
        string Format(MessageException message);
    }
}
