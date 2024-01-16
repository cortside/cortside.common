using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cortside.Common.Testing {
    public class LoremIpsumGenerator {
        private const string text =
@"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras nulla neque, efficitur eget elit ac, feugiat semper mauris. Vestibulum aliquam lorem elit, sed sollicitudin lectus eleifend vitae. Duis vestibulum nec orci sed lacinia. Aliquam ut justo nulla. Proin ut lectus velit. Fusce auctor, mauris quis dapibus vehicula, augue leo volutpat libero, in consectetur diam leo non felis. Curabitur volutpat consequat leo non accumsan. Proin nec faucibus nisl.

Donec accumsan diam sed velit aliquam vestibulum. Aliquam venenatis mi massa, in lacinia massa molestie in. Quisque sed ante nec odio facilisis euismod feugiat nec nibh. Quisque quis orci varius, viverra lacus vel, accumsan mi. Suspendisse sit amet felis vestibulum, tempus dolor in, tincidunt nisl. Suspendisse eu nisi elementum, porttitor justo non, gravida quam. Fusce lobortis consectetur ultricies. In pharetra, nulla vel fringilla lacinia, justo nulla viverra orci, quis elementum diam arcu ut sapien. In sit amet lacinia enim, sit amet facilisis eros. Phasellus vulputate odio nec mauris semper, aliquet auctor dui lobortis. Quisque sapien nisi, tempus et elit eget, sodales auctor justo.

In vitae ultrices metus. Etiam efficitur rhoncus eros sit amet ornare. Vivamus leo dolor, convallis eget ornare semper, interdum ut ex. Pellentesque nec dui maximus, dignissim eros sit amet, mollis justo. Phasellus et nibh diam. Mauris non urna non orci varius luctus id eu mi. Sed ut feugiat erat. Vestibulum faucibus dolor ante, eu aliquet orci hendrerit faucibus. Proin tincidunt enim ut magna ultricies, nec varius lectus cursus. Quisque justo eros, faucibus non odio a, hendrerit lobortis dui. Maecenas lobortis feugiat pellentesque. Proin leo est, semper nec nunc tempus, faucibus interdum leo. Maecenas quis gravida ex, sit amet sodales nulla. Nam malesuada viverra turpis, vitae sodales nibh imperdiet vitae.

Vivamus interdum at purus non mattis. Proin pulvinar commodo luctus. Aenean risus quam, lacinia id posuere eget, vehicula nec est. Vivamus pretium hendrerit nunc. Sed nec ligula suscipit, molestie tellus id, elementum sapien. In sodales sem ut lectus tempor congue. Integer ullamcorper tristique sapien sit amet rhoncus. Morbi lacinia, urna vel rutrum laoreet, nisi sem feugiat elit, ac varius enim nisl vel mi. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Sed finibus eros eget libero tempor, id consequat enim luctus. Sed a magna nec nisl vestibulum bibendum non quis nunc. Pellentesque risus metus, posuere ac maximus non, convallis vitae ex. Mauris finibus malesuada arcu vitae ullamcorper.

Aliquam a libero sed orci finibus pulvinar nec eget leo. Mauris ut dignissim nibh. Integer malesuada tempus pulvinar. In elementum placerat nisl, ornare dapibus purus porttitor vitae. Donec at lacus at tellus gravida vestibulum nec eleifend lacus. Curabitur malesuada lorem non gravida pharetra. Donec lacus ligula, tincidunt bibendum ultricies placerat, pellentesque id elit. Nulla scelerisque sodales tempus. Quisque maximus non urna eget consequat. Nunc quis tincidunt augue, a aliquam nulla. Integer dolor ligula, lacinia eget orci id, consequat vulputate nibh. Integer congue tellus aliquet felis ullamcorper, quis molestie neque convallis. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam iaculis pharetra mi, eu sodales mi euismod eget.
";
        private readonly List<string> words = new List<string>();
        private readonly List<string> sentences = new List<string>();
        private readonly List<string> paragraphs;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoremIpsumGenerator"/> class.
        /// </summary>
        public LoremIpsumGenerator() {
            paragraphs = Regex.Split(text.ReplaceLineEndings(), @"[\n]+", RegexOptions.Compiled).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToList();

            foreach (var s in paragraphs) {
                sentences.AddRange(Regex.Split(s, @"[.]+", RegexOptions.Compiled).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim() + ".").ToList());
            }

            foreach (var s in paragraphs) {
                words.AddRange(Regex.Split(s, @"\W+").Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToList());
            }
        }

        /// <summary>
        /// Gets a loremipsum string with length of given length of letters.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>A lorem ipsum string with a given length.</returns>
        public string GetLetters(int length) {
            return text.Substring(0, length);
        }

        /// <summary>
        /// Gets a loremipsum string of given length of words.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>a lorem ipsum string</returns>
        public string GetWords(int length) {
            return string.Join(" ", words.Take(length));
        }

        /// <summary>
        /// Gets a loremipsum string of given length of sentences.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>a lorem ipsum string</returns>
        public string GetSentences(int length) {
            return string.Join(" ", sentences.Take(length));
        }

        /// <summary>
        /// Gets a loremipsum string of given length of paragraphs.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>a lorem ipsum string</returns>
        public string GetParagraphs(int length) {
            return string.Join("\n\n", paragraphs.Take(length));
        }
    }
}
