using System;
using System.Collections.Generic;
using System.Text;
using TwoFactorAuthNet;

namespace frznUpload.Server
{

    public static class TwoFactorHandler
    {
        private static TwoFactorAuth tfa = new TwoFactorAuth("fritzen.ml");

        public static string CreateSecret()
        {
            return tfa.CreateSecret(512, CryptoSecureRequirement.RequireSecure);
        }

        public static bool Verify(string secret, string code)
        {
            return tfa.VerifyCode(secret, code);
        }

        public static string GenerateQrCode(string username, string secret)
        {
            return tfa.GetQrCodeImageAsDataUri(username, secret, 256);
        }
        
    }
}
