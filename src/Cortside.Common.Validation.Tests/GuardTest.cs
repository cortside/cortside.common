using System;
using Cortside.Common.Messages.MessageExceptions;
using Cortside.Common.Messages.Models;
using Xunit;

namespace Cortside.Common.Validation.Tests {
    public class GuardTest {
        [Fact]
        public void Against() {
            string s = null;
            Assert.Throws<ArgumentNullException>(() => Guard.Against(() => s == null, () => throw new ArgumentNullException(nameof(s), $"{nameof(s)} is null")));
        }

        [Fact]
        public void AgainstNull() {
            string s = null;
            Assert.Throws<ArgumentException>(() => Guard.From.Null(s, nameof(s), $"{nameof(s)} was null"));

            ErrorModel o = null;
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
