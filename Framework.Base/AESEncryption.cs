// Copyright 2016-2040 Nino Crudele
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#region Usings

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

#endregion

namespace SnapGate.Framework.Base
{
    public class AesEncryption
    {
        private static string SecurityFile = "KeyFile.gck";

        public static bool CreateSecurityKey_Aes()
        {
            byte[] Key = new byte[32];
            byte[] IV = new byte[16];
            bool ret = false;

            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                byte[] keyContent = aesAlg.Key.Concat(aesAlg.IV).ToArray();
                File.WriteAllBytes(GetSecurityFileName_Aes(), keyContent);
                string[] filesConf = Directory.GetFiles(ConfigurationBag.Configuration.BaseDirectory, "*.cfg");
                foreach (var item in filesConf)
                {
                    byte[] content = File.ReadAllBytes(item);
                    byte[] contentCrypted = EncryptByteToBytes_Aes(content);
                    File.WriteAllBytes(item, contentCrypted);
                }
            }

            ret = true;


            return ret;
        }

        public static bool DeleteSecurityKey_Aes()
        {
            byte[] Key = new byte[32];
            byte[] IV = new byte[16];
            bool ret = false;


            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                byte[] keyContent = aesAlg.Key.Concat(aesAlg.IV).ToArray();
                File.WriteAllBytes(GetSecurityFileName_Aes(), keyContent);
                string[] filesConf = Directory.GetFiles(ConfigurationBag.Configuration.BaseDirectory, "*.cfg");
                foreach (var item in filesConf)
                {
                    byte[] content = File.ReadAllBytes(item);
                    byte[] contentCrypted = EncryptByteToBytes_Aes(content);
                    File.WriteAllBytes(item, contentCrypted);
                }
            }

            ret = true;


            return ret;
        }

        public static bool SecurityOn_Aes()
        {
            return File.Exists(GetSecurityFileName_Aes());
        }

        public static string GetSecurityFileName_Aes()
        {
            string secFile = Path.Combine(ConfigurationBag.Configuration.BaseDirectory, SecurityFile);
            return secFile;
        }

        public static byte[] GetSecurityContent_Aes()
        {
            return File.ReadAllBytes(GetSecurityFileName_Aes());
        }

        public static byte[] EncryptByteToBytes_Aes(byte[] byteContent)
        {
            byte[] encrypted = null;


            // Create an AesManaged object
            // with the specified key and IV.

            using (AesManaged aesAlg = new AesManaged())
            {
                byte[] Key = new byte[32];
                byte[] IV = new byte[16];

                byte[] contentKey = GetSecurityContent_Aes();
                Array.Copy(contentKey, 0, Key, 0, 32);
                Array.Copy(contentKey, 32, IV, 0, 16);

                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(byteContent, 0, byteContent.Length);
                        csEncrypt.FlushFinalBlock();
                        encrypted = msEncrypt.ToArray();
                        msEncrypt.Close();
                        csEncrypt.Close();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static byte[] DecryptByteFromBytes_Aes(byte[] cipherText)
        {
            // the decrypted byte array.
            byte[] deCipherText = new byte[cipherText.Length];
            ;

            // Create an AesManaged object
            // with the specified key and IV.

            using (AesManaged aesAlg = new AesManaged())
            {
                byte[] Key = new byte[32];
                byte[] IV = new byte[16];
                byte[] contentKey = GetSecurityContent_Aes();
                Array.Copy(contentKey, 0, Key, 0, 32);
                Array.Copy(contentKey, 32, IV, 0, 16);

                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        csDecrypt.Read(deCipherText, 0, deCipherText.Length);
                        msDecrypt.Close();
                        csDecrypt.Close();
                    }
                }
            }

            return deCipherText;
        }
    }
}