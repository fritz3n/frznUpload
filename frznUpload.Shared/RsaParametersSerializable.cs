using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace frznUpload.Shared
{
    [Serializable]
    class RsaParametersSerializable : ISerializable
    {
        public RSAParameters RsaParameters { get; private set; }

        public RsaParametersSerializable(RSAParameters rsaParameters)
        {
            RsaParameters = rsaParameters;
        }

        public RsaParametersSerializable(SerializationInfo information, StreamingContext context)
        {
            RsaParameters = new RSAParameters()
            {
                D = (byte[])information.GetValue("D", typeof(byte[])),
                DP = (byte[])information.GetValue("DP", typeof(byte[])),
                DQ = (byte[])information.GetValue("DQ", typeof(byte[])),
                Exponent = (byte[])information.GetValue("Exponent", typeof(byte[])),
                InverseQ = (byte[])information.GetValue("InverseQ", typeof(byte[])),
                Modulus = (byte[])information.GetValue("Modulus", typeof(byte[])),
                P = (byte[])information.GetValue("P", typeof(byte[])),
                Q = (byte[])information.GetValue("Q", typeof(byte[]))
            };
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("D", RsaParameters.D);
            info.AddValue("DP", RsaParameters.DP);
            info.AddValue("DQ", RsaParameters.DQ);
            info.AddValue("Exponent", RsaParameters.Exponent);
            info.AddValue("InverseQ", RsaParameters.InverseQ);
            info.AddValue("Modulus", RsaParameters.Modulus);
            info.AddValue("P", RsaParameters.P);
            info.AddValue("Q", RsaParameters.Q);
        }
    }
    
    static class RsaRExtensions
    {
        public static RsaParametersSerializable ToSerializable(this RSAParameters rsaParameters)
        {
            return new RsaParametersSerializable(rsaParameters);
        }
    }
}
