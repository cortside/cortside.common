using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Cortside.Common.Cryptography {
    public class EncryptionService : IEncryptionService {
        private readonly byte[] aesKey;
        private readonly byte[] aesIV;

        public EncryptionService(string secret) {
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] passwordBytes = UE.GetBytes(secret);
            aesKey = SHA256.Create().ComputeHash(passwordBytes);
            aesIV = MD5.Create().ComputeHash(passwordBytes);
        }

        public string EncryptObject<T>(T objectToEncrypt) {
            string objectString = JsonConvert.SerializeObject(objectToEncrypt);
            return EncryptString(objectString);
        }

        public T DecryptObject<T>(string cipherText) {
            string decryptedString = DecryptString(cipherText);
            var response = JsonConvert.DeserializeObject<T>(decryptedString);
            if (response == null) {
                throw new JsonSerializationException("Unable to deserialize string");
            }
            return response;
        }

        public string EncryptString(string plainText) {
            // Check arguments.
            if (string.IsNullOrEmpty(plainText)) {
                throw new ArgumentNullException(nameof(plainText));
            }
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create()) {
                aesAlg.Key = aesKey;
                aesAlg.IV = aesIV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream()) {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream as Base64 string
            return Convert.ToBase64String(encrypted);
        }

        public string DecryptString(string text) {
            var cipherText = Convert.FromBase64String(text);

            // Check arguments.
            if (cipherText == null || cipherText.Length == 0) {
                throw new ArgumentNullException(nameof(text));
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create()) {
                aesAlg.Key = aesKey;
                aesAlg.IV = aesIV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText)) {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
