using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Foundation;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Util.Http;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Apollo.Tests
{
    public class NetworkInterfaceManagerTest
    {
        [Fact]
        public void IsInSubnet()
        {
          var isInSubnet=  NetworkInterfaceManager.IsInSubnet("172.2.1.1","172.0.0.0/8");

            Assert.True(isInSubnet);

        }
    }
}
