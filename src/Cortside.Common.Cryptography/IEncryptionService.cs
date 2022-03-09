namespace Cortside.Common.Cryptography {
    public interface IEncryptionService {
        T DecryptObject<T>(string cipherText);
        string DecryptString(string text);
        string EncryptObject<T>(T objectToEncrypt);
        string EncryptString(string plainText);
    }
}