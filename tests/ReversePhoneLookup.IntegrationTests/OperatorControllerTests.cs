using Newtonsoft.Json;
using ReversePhoneLookup.Models.ViewModels;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReversePhoneLookup.IntegrationTests
{
    [Collection("DbFixture user")]
    public class OperatorControllerTests : 
        IClassFixture<CustomWebApplicationFactory<StartupSUT>>
    {
        private readonly HttpClient client;

        public OperatorControllerTests(CustomWebApplicationFactory<StartupSUT> factory)
        {
            this.client = factory.CreateClient();
        }

        [Fact]
        public async Task AddOperator_ValidData_ShouldReturn201()
        {
            using (var fixture = new DbFixture())
            {
                var url = "/operators";

                var @operator = new OperatorViewModelIn()
                {
                    Mcc = "420",
                    Mnc = "420",
                    Name = "SkrSkr"
                };

                var json = JsonConvert.SerializeObject(@operator);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, data);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }
        }
    }
}
