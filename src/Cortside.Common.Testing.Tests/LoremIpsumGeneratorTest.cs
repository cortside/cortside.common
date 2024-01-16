using Xunit;

namespace Cortside.Common.Testing.Tests {
    public class LoremIpsumGeneratorTest {
        private readonly LoremIpsumGenerator generator;

        public LoremIpsumGeneratorTest() {
            generator = new LoremIpsumGenerator();
        }

        [Fact]
        public void ShouldGetLetters() {
            var s = generator.GetLetters(20);

            Assert.Equal("Lorem ipsum dolor si", s);
        }

        [Fact]
        public void ShouldGetWords() {
            var s = generator.GetWords(6);

            Assert.Equal("Lorem ipsum dolor sit amet consectetur", s);
        }

        [Fact]
        public void ShouldGetSentences() {
            var s = generator.GetSentences(2);

            Assert.Equal("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras nulla neque, efficitur eget elit ac, feugiat semper mauris.", s);
        }

        [Fact]
        public void ShouldGetParagraphs() {
            var s = generator.GetParagraphs(1);

            Assert.Equal("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras nulla neque, efficitur eget elit ac, feugiat semper mauris. Vestibulum aliquam lorem elit, sed sollicitudin lectus eleifend vitae. Duis vestibulum nec orci sed lacinia. Aliquam ut justo nulla. Proin ut lectus velit. Fusce auctor, mauris quis dapibus vehicula, augue leo volutpat libero, in consectetur diam leo non felis. Curabitur volutpat consequat leo non accumsan. Proin nec faucibus nisl.", s);
        }
    }
}
