using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ClinicApp.Core
{
    public static class Security
    {
        // AES-256 Key and IV. 
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("ClinicApp2026!SecretKey12345678"); // 32 bytes
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("ClinicAppIV12345"); // 16 bytes

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key; 
                aes.IV = IV;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (StreamWriter sw = new StreamWriter(cs)) 
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText)) return encryptedText;
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key; 
                    aes.IV = IV;
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(encryptedText)))
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (StreamReader sr = new StreamReader(cs)) 
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch 
            { 
                return "[DECRYPTION FAILED]"; 
            }
        }
    }
}
