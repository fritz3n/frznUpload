using frznUpload.Server;
using System;
using Xunit;

namespace ServerTests
{
    public class TwoFaTests
    {
        [Fact]
        public void CanGenertaeScret()
        {
            TwoFactorHandler.CreateSecret();
        }
    }
}
