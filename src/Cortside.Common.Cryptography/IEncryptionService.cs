namespace Cortside.Common.Cryptography {
    public interface IEncryptionService {
        T DecryptObject<T>(string cipherText) where T : class;
        string DecryptString(string text);
        string EncryptObject<T>(T objectToEncrypt) where T : class;
        string EncryptString(string plainText);
    }
}
