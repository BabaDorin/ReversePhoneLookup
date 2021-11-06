using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReversePhoneLookup.Models.Models.Entities;
using ReversePhoneLookup.Models.ViewModels;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReversePhoneLookup.IntegrationTests
{
    [Collection("DbFixture user")]
    public class PhoneControllerTests : IClassFixture<CustomWebApplicationFactory<StartupSUT>>
    {
        private readonly HttpClient client;

        public PhoneControllerTests(CustomWebApplicationFactory<StartupSUT> factory)
        {
            this.client = factory.CreateClient();
        }

        [Fact]
        public async Task Lookup_ValidData_ShouldReturn200()
        {
            using (var fixture = new DbFixture())
            {
                fixture.DbContext.Phones.Add(new Phone()
                {
                    Value = "+37367123456",
                    Operator = new Operator()
                    {
                        Mcc = "123",
                        Mnc = "99",
                        Name = "Test"
                    },
                    Contacts = new List<Contact>()
                    {
                        new Contact() { Name = "User" }
                    }
                });
                await fixture.DbContext.SaveChangesAsync();

                string url = "/lookup?phone=67123456";

                var response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                JToken json = JToken.Parse(jsonResponse);

                Assert.Equal("+37367123456", json["phone"]);
            }
        }

        [Fact]
        public async Task AddPhone_ValidData_ShouldReturn201()
        {
            using (var fixture = new DbFixture())
            {
                fixture.DbContext.Operators.Add(new Operator()
                {
                    Id = 1,
                    Mcc = "123",
                    Mnc = "99",
                    Name = "Test"
                });

                await fixture.DbContext.SaveChangesAsync();

                var phone = new PhoneViewModelIn()
                {
                    Value = "+37367123456",
                    OperatorId = 1,
                    Contacts = new List<ContactViewModelIn>()
                {
                    new ContactViewModelIn()
                    {
                        Name = "New Contact",
                        Phone = new PhoneViewModelIn()
                        {
                            Value = "+37367123456",
                            OperatorId = 1
                        }
                    }
                }
                };

                var json = JsonConvert.SerializeObject(phone);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                string url = "/phones";
                var response = await client.PostAsync(url, data);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }
        }
    }
}
