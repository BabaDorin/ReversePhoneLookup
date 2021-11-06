﻿using Newtonsoft.Json.Linq;
using ReversePhoneLookup.Models.Models.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ReversePhoneLookup.IntegrationTests
{
    /// <summary>
    /// DONE: Add the functionality of inserting records to datase.
    /// TODO: Expose Endpoint
    /// TODO: Write Unit Tests
    /// TODO: Write Integration tests
    /// </summary>

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
    }
}
