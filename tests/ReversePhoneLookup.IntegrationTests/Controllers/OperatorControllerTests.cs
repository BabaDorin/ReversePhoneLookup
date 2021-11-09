using Newtonsoft.Json;
using ReversePhoneLookup.IntegrationTests.Shared;
using ReversePhoneLookup.Models.Models.Entities;
using ReversePhoneLookup.Models.Requests;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReversePhoneLookup.IntegrationTests.Controllers
{
    [Collection("DbFixture user")]
    public class OperatorControllerTests :
        IClassFixture<CustomWebApplicationFactory<StartupSUT>>
    {
        private readonly HttpClient client;

        public OperatorControllerTests(CustomWebApplicationFactory<StartupSUT> factory)
        {
            client = factory.CreateClient();
        }

        [Fact]
        public async Task AddOperator_ValidData_ShouldReturn201()
        {
            using (var fixture = new DbFixture())
            {
                var request = new CreateOperatorRequest()
                {
                    Mcc = "420",
                    Mnc = "42",
                    Name = "SkrSkr"
                };

                var url = "/operators";
                var response = await client.PostAsync(url, Helpers.ToStringContent(request));

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                var newOperator = fixture.DbContext.Operators
                    .Where(o => o.Mnc == request.Mnc && o.Mcc == request.Mcc && o.Name == request.Name);

                Assert.Single(newOperator);
            }
        }

        [Fact]
        public async Task AddOperator_ExistentOperator_ShouldReturn409()
        {
            using (var fixture = new DbFixture())
            {
                var existingOperator = new Operator()
                {
                    Mcc = "420",
                    Mnc = "42",
                    Name = "SkrSkr"
                };

                await fixture.DbContext.Operators.AddAsync(existingOperator);
                await fixture.DbContext.SaveChangesAsync();

                var request = new CreateOperatorRequest()
                {
                    Mcc = existingOperator.Mcc,
                    Mnc = existingOperator.Mnc,
                    Name = existingOperator.Name
                };

                var url = "/operators";
                var response = await client.PostAsync(url, Helpers.ToStringContent(request));

                Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            }
        }
    }
}
