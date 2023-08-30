namespace Cortside.Common.Messages.Tests.Exceptions {
    /// <summary>
    /// Simple message for testing and to show how messages are created.
    /// </summary>
    public class TestMessage : MessageException {
        public TestMessage(string param1, string param2) : base("First parameter is {0}. Second parameter is {1}.", null, param1, param2) {
        }
    }
}
