using Microsoft.Extensions.Configuration;
using ReversePhoneLookup.Web;

namespace ReversePhoneLookup.IntegrationTests.Shared
{
    public class StartupSUT : Startup
    {
        public StartupSUT(IConfiguration configuration) : base(configuration) { }
    }
}
