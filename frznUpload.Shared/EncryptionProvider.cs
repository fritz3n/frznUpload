using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace frznUpload.Shared
{
    public class EncryptionProvider : IDisposable
    {
        public RSACryptoServiceProvider LocalKey { get; set; }
        public RSACryptoServiceProvider RemoteKey { get; set; }
        RijndaelManaged aes = new RijndaelManaged();

        ~EncryptionProvider()
        {
            Dispose();
        }

        public EncryptionProvider(RSACryptoServiceProvider localKey = null)
        {
            if (localKey == null)
                localKey = RetrieveKey();

            LocalKey = localKey;

            aes.KeySize = 128;
        }

        public static RSACryptoServiceProvider RetrieveKey()
        {
            CspParameters csp = new CspParameters
            {
                KeyContainerName = "frznUploadLocalKey"
            };

            return new RSACryptoServiceProvider(2048, csp);
        }

        public byte[] DecryptLocal(byte[] data)
        {
            byte ivLength = data[0];
            byte keyLength = data[1];

            byte[] ivEnc = new byte[ivLength];
            byte[] keyEnc = new byte[keyLength];

            Array.Copy(data, 2, ivEnc, 0, ivLength);
            Array.Copy(data, 2 + ivLength, keyEnc, 0, keyLength);

            byte[] encrypted = new byte[data.Length - 2 - ivLength - keyLength];

            Array.Copy(data, 2 + ivLength + keyLength, encrypted, 0, encrypted.Length);

            aes.Key = LocalKey.Decrypt(keyEnc, true);
            aes.IV = LocalKey.Decrypt(ivEnc, true);

            var dec = aes.CreateDecryptor();

            byte[] buffer;
            int decryptedCount;

            using (MemoryStream from = new MemoryStream(encrypted))
            {
                using (CryptoStream reader = new CryptoStream(from, dec, CryptoStreamMode.Read))
                {
                    buffer = new byte[encrypted.Length];
                    decryptedCount = reader.Read(buffer, 0, buffer.Length);
                }
            }

            byte[] decrypted = new byte[decryptedCount];

            Array.Copy(buffer, decrypted, decryptedCount);

            return decrypted;
        }

        public byte[] EncryptRemote(byte[] data)
        {
            aes.GenerateIV();
            aes.GenerateKey();

            var enc = aes.CreateDecryptor();

            byte[] encrypted;

            using (MemoryStream to = new MemoryStream())
            {
                using (CryptoStream writer = new CryptoStream(to, enc, CryptoStreamMode.Write))
                {
                    writer.Write(data, 0, data.Length);
                    writer.FlushFinalBlock();
                    encrypted = to.ToArray();
                }
            }

            byte[] final = new byte[2 + aes.IV.Length + aes.Key.Length + encrypted.Length];

            byte[] iv = RemoteKey.Encrypt(aes.IV, true);
            byte[] key = RemoteKey.Encrypt(aes.Key, true);

            final[0] = (byte)iv.Length;
            final[1] = (byte)key.Length;

            iv.CopyTo(final, 2);
            key.CopyTo(final, iv.Length + 2);
            encrypted.CopyTo(final, aes.Key.Length + 2);

            return encrypted;
        }

        public void SetRemoteKey(byte[] data)
        {
            var deSerializer = new BinaryFormatter();
            RSAParameters param;

            using (Stream stream = new MemoryStream(data))
            {
                param = (RSAParameters)deSerializer.Deserialize(stream);
            }

            RemoteKey.ImportParameters(param);
        }

        public void SetLocalKey(byte[] data)
        {
            var deSerializer = new BinaryFormatter();
            RSAParameters param;
            using (Stream stream = new MemoryStream(data))
            {

                param = (RSAParameters)deSerializer.Deserialize(stream);
            }
            LocalKey.ImportParameters(param);
        }

        public byte[] GetLocalKey(bool exportPrivate = false)
        {
            var deSerializer = new BinaryFormatter();
            byte[] data;

            using (Stream stream = new MemoryStream())
            {
                deSerializer.Serialize(stream, LocalKey.ExportParameters(exportPrivate));

                data = new byte[stream.Length];

                stream.Read(data, 0, (int)stream.Length);
            }

            return data;
        }

        //private byte[] KeyToByte(RSAParameters key)
        //{
        //
        //}

        public void Dispose()
        {
            LocalKey.Dispose();
            RemoteKey.Dispose();
            aes.Dispose();
        }
    }
}
