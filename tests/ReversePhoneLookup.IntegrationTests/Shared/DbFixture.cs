﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ReversePhoneLookup.Models;
using System;

namespace ReversePhoneLookup.IntegrationTests.Shared
{
    public class DbFixture : IDisposable
    {
        public DbFixture()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var dbContextOptions = new DbContextOptionsBuilder<PhoneLookupContext>()
                .UseNpgsql(config.GetConnectionString("Default"))
                .Options;

            DbContext = new PhoneLookupContext(dbContextOptions);

            DbContext.Database.EnsureCreated();
        }

        public PhoneLookupContext DbContext { get; }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
        }
    }
}
