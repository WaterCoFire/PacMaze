using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Entity.Map.Utility {
    /**
     * Responsible for encrypting and decrypting map data.
     */
    public static class AesHelper {
        // 32-byte Key (256-bit)
        // This Key is fixed. The thing that changes is the IV.
        private const string Key = "PacMazeGameByWaterCoFireAESKey32";

        /**
         * Encrypts the plain text.
         * A random Initialisation Vector (IV) will be generated
         * and the IV will be written to the beginning of the cipher text.
         * This code was written with the help of ChatGPT
         */
        public static byte[] EncryptWithRandomIv(string plainText) {
            using (Aes aes = Aes.Create()) {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.GenerateIV(); // Each time a new IV is generated
                byte[] iv = aes.IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                using (var ms = new MemoryStream()) {
                    ms.Write(iv, 0, iv.Length); // Write the IV to the stream first

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs)) {
                        sw.Write(plainText);
                    }

                    return ms.ToArray(); // Return: IV(16 bytes) + cipher text
                }
            }
        }

        /**
         * Decrypts the cipher text.
         * Read the IV from the first 16 bytes and then decrypt the cipher text.
         * This code was written with the help of ChatGPT
         */
        public static string DecryptWithIv(byte[] encryptedData) {
            using (Aes aes = Aes.Create()) {
                aes.Key = Encoding.UTF8.GetBytes(Key);

                byte[] iv = new byte[16];
                Array.Copy(encryptedData, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (var decrypter = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length))
                using (var cs = new CryptoStream(ms, decrypter, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs)) {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}