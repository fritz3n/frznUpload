using frznUpload.Server;
using System;
using Xunit;

namespace ServerTests
{
    public class TowFaTests
    {
        [Fact]
        public void CanGenertaeScret()
        {
            TowFactorHandler.CreateSecret();
        }
    }
}
