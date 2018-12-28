using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace frznUpload.Shared
{
    public class Challenge
    {
        public RSAParameters? Key { get; set; }
        private byte[] challengeBytes;


        public Challenge()
        {

        }

        public byte[] GetThumbprint()
        {
            if (Key == null)
                throw new ArgumentException("No key was set!");

            var k = Key.Value;

            byte[] hash;

            using (SHA512CryptoServiceProvider sha = new SHA512CryptoServiceProvider())
            {
                byte[] data = new byte[k.Exponent.Length + k.Modulus.Length];
                Array.Copy(k.Exponent, data, k.Exponent.Length);
                Array.Copy(k.Modulus, 0, data, k.Exponent.Length, k.Modulus.Length);

                hash = sha.ComputeHash(data);
            }

            return hash;
        }

        public byte[][] GetPublicComponents()
        {
            if (Key == null)
                throw new ArgumentException("No key was set!");

            return new byte[][] { Key.Value.Exponent, Key.Value.Modulus };
        }
        
        public void SetPublicComponents(byte[] exponent, byte[] modulus)
        {
            Key = new RSAParameters() { Exponent = exponent, Modulus = modulus};
        }

        public void LoadKey(Stream keyStream)
        {
            var bin = new BinaryFormatter();
            
            Key = ((RsaParametersSerializable)bin.Deserialize(keyStream)).RsaParameters;
        }

        public void GenerateKey(int bits)
        {
            using (var crypt = new RSACryptoServiceProvider(bits))
            {
                Key = crypt.ExportParameters(true);
            }
        }

        public void ExportKey(Stream keyStream)
        {
            var bin = new BinaryFormatter();

            bin.Serialize(keyStream, Key.Value.ToSerializable());
        }

        public byte[] GenerateChallenge(int randomBytes)
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                challengeBytes = new byte[8 + randomBytes];

                long time = DateTime.Now.Ticks;
                byte[] timeStamp = BitConverter.GetBytes(time);

                Array.Copy(timeStamp, challengeBytes, 8);

                rng.GetBytes(challengeBytes, 8, randomBytes);

                using (SHA512CryptoServiceProvider sha = new SHA512CryptoServiceProvider())
                {
                    challengeBytes = sha.ComputeHash(challengeBytes);
                }

                

                return challengeBytes;
            }
        }

        public byte[] SignChallenge(byte[] challenge)
        {
            if (Key == null)
                throw new ArgumentException("No key was set!");

            using (var crypt = new RSACryptoServiceProvider())
            {
                crypt.ImportParameters((RSAParameters)Key);
                if (crypt.PublicOnly)
                    throw new ArgumentException("The loaded key is not suitable for this operation");
                
                var ou = crypt.SignHash(challenge, CryptoConfig.MapNameToOID("SHA512"));

                return ou;
            }
        }

        public bool ValidateChallenge(byte[] response)
        {
            if (Key == null)
                throw new ArgumentException("No key was set!");

            using (var crypt = new RSACryptoServiceProvider())
            {
                crypt.ImportParameters((RSAParameters)Key);

                return crypt.VerifyHash(challengeBytes, CryptoConfig.MapNameToOID("SHA512"), response);
            }
        }
    }
}
