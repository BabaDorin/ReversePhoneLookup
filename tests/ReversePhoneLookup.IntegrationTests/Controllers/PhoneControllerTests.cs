using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReversePhoneLookup.IntegrationTests.Shared;
using ReversePhoneLookup.Models.Models.Entities;
using ReversePhoneLookup.Models.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReversePhoneLookup.IntegrationTests.Controllers
{
    [Collection("DbFixture user")]
    public class PhoneControllerTests : IClassFixture<CustomWebApplicationFactory<StartupSUT>>
    {
        private readonly HttpClient client;

        public PhoneControllerTests(CustomWebApplicationFactory<StartupSUT> factory)
        {
            client = factory.CreateClient();
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
        public async Task AddPhone_NewPhone_ShouldReturn201()
        {
            using (var fixture = new DbFixture())
            {
                var existentOperator = new Operator()
                {
                    Id = 123,
                    Mcc = "123",
                    Mnc = "99",
                    Name = "Test"
                };
                fixture.DbContext.Operators.Add(existentOperator);

                await fixture.DbContext.SaveChangesAsync();

                var request = new UpsertPhoneRequest()
                {
                    Value = "+37367123456",
                    OperatorId = existentOperator.Id,
                    Contact = new CreateContactRequest { Name = "NewContact" }
                };

                string url = "/phones";
                var response = await client.PutAsync(url, Helpers.ToStringContent(request));
                
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                var newPhone = fixture.DbContext.Phones
                    .Where(p =>
                        p.Value == request.Value
                        && p.OperatorId == request.OperatorId
                        && p.Contacts.First().Name == request.Contact.Name);
                
                Assert.Single(newPhone);
            }
        }

        [Fact]
        public async Task AddPhone_ExistentPhone_NewContact_ShouldReturn204()
        {
            using (var fixture = new DbFixture())
            {
                var existentPhone = new Phone()
                {
                    Value = "+37367123456",
                    Operator = new Operator()
                    {
                        Id = 123,
                        Mcc = "123",
                        Mnc = "99",
                        Name = "Test"
                    }
                };
                fixture.DbContext.Phones.Add(existentPhone);

                await fixture.DbContext.SaveChangesAsync();

                var request = new UpsertPhoneRequest()
                {
                    Value = existentPhone.Value,
                    OperatorId = (int)existentPhone.OperatorId,
                    Contact = new CreateContactRequest { Name = "NewContact" }
                };

                string url = "/phones";
                var response = await client.PutAsync(url, Helpers.ToStringContent(request));

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var newPhone = fixture.DbContext.Phones
                    .Where(p =>
                        p.Value == request.Value
                        && p.OperatorId == request.OperatorId
                        && p.Contacts.First().Name == request.Contact.Name);

                Assert.Single(newPhone);
            }
        }

        [Fact]
        public async Task AddPhone_ExistentPhone_ExistentContact_ShouldReturn409() // 409 = conflict
        {
            using (var fixture = new DbFixture())
            {
                var existentContact = new Contact()
                {
                    Name = "Existent Contact"
                };

                var existentPhone = new Phone()
                {
                    Value = "+37367123456",
                    Operator = new Operator()
                    {
                        Id = 123,
                        Mcc = "123",
                        Mnc = "99",
                        Name = "Test"
                    },
                    Contacts = new List<Contact>() { existentContact }
                };
                fixture.DbContext.Phones.Add(existentPhone);

                await fixture.DbContext.SaveChangesAsync();

                var request = new UpsertPhoneRequest()
                {
                    Value = existentPhone.Value,
                    OperatorId = (int)existentPhone.OperatorId,
                    Contact = new CreateContactRequest { Name = existentContact.Name }
                };

                string url = "/phones";
                var response = await client.PutAsync(url, Helpers.ToStringContent(request));

                Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            }
        }

        [Fact]
        public async Task AddPhone_InvalidPhoneNumber_ShouldReturn400()
        {
            using (var fixture = new DbFixture())
            {
                var existentPhone = new Phone()
                {
                    Value = "00000000",
                    Operator = new Operator()
                    {
                        Id = 123,
                        Mcc = "123",
                        Mnc = "99",
                        Name = "Test"
                    },
                };
                fixture.DbContext.Phones.Add(existentPhone);

                await fixture.DbContext.SaveChangesAsync();

                var request = new UpsertPhoneRequest()
                {
                    Value = "00000000",
                    OperatorId = (int)existentPhone.OperatorId,
                };

                string url = "/phones";
                var response = await client.PutAsync(url, Helpers.ToStringContent(request));

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task AddPhone_UnexistentOperatorId_ShouldReturn404()
        {
            using (var fixture = new DbFixture())
            {
                await fixture.DbContext.SaveChangesAsync();

                var request = new UpsertPhoneRequest()
                {
                    Value = "+37367123456",
                    OperatorId = 1 // does not exist
                };

                string url = "/phones";
                var response = await client.PutAsync(url, Helpers.ToStringContent(request));

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}
