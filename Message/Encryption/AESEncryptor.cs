using System;
using System.IO;
using System.Security.Cryptography;

namespace Message.Encryption
{
    public static class AESEncryptor
    {
        private static byte[] Key
            = new byte[32] { 147, 132, 16, 132, 16, 125, 36, 150, 32, 9, 252, 4, 11, 236, 212, 97, 116, 22, 239, 139, 44, 200, 47, 252, 250, 217, 247, 12, 3, 116, 1, 71 };
        private static byte[] IV
            = new byte[16] { 37, 52, 91, 37, 213, 38, 140, 90, 183, 22, 216, 0, 0, 0, 1, 128 };
        
        public static byte[] encryptPassword(string plainPassword)
        {
            if (plainPassword == null || plainPassword.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainPassword);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            
            return encrypted;

        }
        
        public static string decryptPassword(byte[] encryptedPassword)
        {
            if (encryptedPassword == null || encryptedPassword.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            
            string plaintext = null;
            
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                
                using (MemoryStream msDecrypt = new MemoryStream(encryptedPassword))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }
}
