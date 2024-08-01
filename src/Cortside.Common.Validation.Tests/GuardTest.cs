using System;
using Cortside.Common.Messages.MessageExceptions;
using Xunit;

namespace Cortside.Common.Validation.Tests {
    public class GuardTest {
        [Fact]
        public void Against() {
            string s = DateTime.Now.Second > 61 ? "what?" : null;
            Assert.Throws<NullReferenceException>(() => Guard.Against(() => s == null, () => throw new NullReferenceException($"{nameof(s)} is null")));
        }

        [Fact]
        public void AgainstNull() {
            string s = null;
            Assert.Throws<ArgumentException>(() => Guard.From.Null(s, nameof(s), $"{nameof(s)} was null"));

            Object o = null;
            Assert.Throws<BadRequestResponseException>(() => Guard.From.Null<BadRequestResponseException>(o, "model is null"));
        }

        [Fact]
        public void AgainstNullOrEmpty() {
            string s = null;
            Assert.Throws<ArgumentException>(() => Guard.From.NullOrEmpty(s, nameof(s), $"{nameof(s)} was null"));
            s = string.Empty;
            Assert.Throws<ArgumentException>(() => Guard.From.NullOrEmpty(s, nameof(s), $"{nameof(s)} was null"));
        }

        [Fact]
        public void AgainstNullOrWhitespace() {
            string s = null;
            Assert.Throws<ArgumentException>(() => Guard.From.NullOrWhitespace(s, nameof(s), $"{nameof(s)} was null"));
            s = string.Empty;
            Assert.Throws<ArgumentException>(() => Guard.From.NullOrWhitespace(s, nameof(s), $"{nameof(s)} was null"));
            s = "   ";
            Assert.Throws<ArgumentException>(() => Guard.From.NullOrWhitespace(s, nameof(s), $"{nameof(s)} was null"));
        }
    }
}
