using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Cortside.Common.Web.Security {
    public class JWT {
        //TODO: warning that this is unused, but need to confirm that it is not being used by any consumers before removing the code
#pragma warning disable CS0649
        private static readonly IDictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>> HashAlgorithms;

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static T DecodeToObject<T>(string token, string key, bool verify = true) {
            var payloadJson = Decode(token, Encoding.UTF8.GetBytes(key), verify);
            return JsonConvert.DeserializeAnonymousType<T>(payloadJson, (T)new object());
        }

        public static string Decode(string token, byte[] key, bool verify = true) {
            var parts = token.Split('.');
            if (parts.Length != 3) {
                throw new ArgumentException("Token must consist from 3 delimited by dot parts");
            }

            var payload = parts[1];
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));

            if (verify) {
                Verify(payload, payloadJson, parts, key);
            }

            return payloadJson;
        }

        private static void Verify(string payload, string payloadJson, string[] parts, byte[] key) {
            var crypto = JWT.Base64UrlDecode(parts[2]);
            var decodedCrypto = Convert.ToBase64String(crypto);

            var header = parts[0];
            var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
            StringReader sr = new StringReader(headerJson);
            JsonTextReader reader = new JsonTextReader(sr);
            var jsonSerializer = new JsonSerializer();
            var headerData = jsonSerializer.Deserialize<Dictionary<string, object>>(reader);
            var algorithm = (string)headerData["alg"];

            var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
            var signatureData = HashAlgorithms[GetHashAlgorithm(algorithm)](key, bytesToSign);
            var decodedSignature = Convert.ToBase64String(signatureData);

            Verify(payloadJson, decodedCrypto, decodedSignature);
        }

        public static void Verify(string payloadJson, string decodedCrypto, string decodedSignature) {
            if (decodedCrypto != decodedSignature) {
                var signatureVerificationException = new SignatureVerificationException("Invalid signature");
                signatureVerificationException.Data.Add("Expected", decodedCrypto);
                signatureVerificationException.Data.Add("Received", decodedSignature);
                throw signatureVerificationException;
            }

            // verify exp claim https://tools.ietf.org/html/draft-ietf-oauth-json-web-token-32#section-4.1.4
            StringReader sr = new StringReader(payloadJson);
            JsonTextReader reader = new JsonTextReader(sr);
            var jsonSerializer = new JsonSerializer();
            var payloadData = jsonSerializer.Deserialize<Dictionary<string, object>>(reader);
            object expObj;
            if (!payloadData.TryGetValue("exp", out expObj) || expObj == null) {
                return;
            }
            int expInt;
            try {
                expInt = Convert.ToInt32(expObj);
            } catch (FormatException) {
                throw new SignatureVerificationException("Claim 'exp' must be an integer.");
            }
            var secondsSinceEpoch = Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds);
            if (secondsSinceEpoch >= expInt) {
                throw new TokenExpiredException("Token has expired.");
            }
        }

        private static string Base64UrlEncode(byte[] input) {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }

        public static byte[] Base64UrlDecode(string input) {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0:
                    break; // No pad chars in this case
                case 2:
                    output += "==";
                    break; // Two pad chars
                case 3:
                    output += "=";
                    break;  // One pad char
                default:
                    throw new FormatException("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }

        private static JwtHashAlgorithm GetHashAlgorithm(string algorithm) {
            switch (algorithm) {
                case "HS256":
                    return JwtHashAlgorithm.HS256;
                case "HS384":
                    return JwtHashAlgorithm.HS384;
                case "HS512":
                    return JwtHashAlgorithm.HS512;
                default:
                    throw new SignatureVerificationException("Algorithm not supported.");
            }
        }

        public enum JwtHashAlgorithm {
            HS256,
            HS384,
            HS512
        }
    }

    public class SignatureVerificationException : Exception {
        public SignatureVerificationException(string message)
            : base(message) {
        }
    }

    public class TokenExpiredException : Exception {
        public TokenExpiredException(string message)
            : base(message) {
        }
    }
}
