using Beis.Htg.VendorSme.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Beis.HelpToGrow.Voucher.Api.Check.Common;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using Beis.HelpToGrow.Voucher.Api.Check;
using Beis.HelpToGrow.Voucher.Api.Check.Interfaces;

namespace Beis.HelpToGrow.Voucher.Api.Check.Tests
{
    [TestFixture]
    public class StartupTests
    {
        private WebApplicationFactory<Startup> _factory;

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("VOUCHER_ENCRYPTION_SALT", "UnitTestSalt");
            Environment.SetEnvironmentVariable("VOUCHER_ENCRYPTION_ITERATION", "1");
            Environment.SetEnvironmentVariable("VOUCHER_ENCRYPTION_INITIAL_VECTOR", "UnitTestVector");
            Environment.SetEnvironmentVariable("VOUCHER_ENCRYPTION_KEY_SIZE", "128");
            Environment.SetEnvironmentVariable("HELPTOGROW_CONNECTIONSTRING", "dbConnectionStr");
            _factory = new WebApplicationFactory<Startup>();
        }

        [Test]
        public void CheckStartup()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                Assert.IsNotNull(serviceProvider.GetService(typeof(ISwaggerProvider)));

                Assert.IsNotNull(serviceProvider.GetService(typeof(IWebHostEnvironment)));
                Assert.IsNotNull(serviceProvider.GetService(typeof(IEncryptionService)));
                Assert.IsNotNull(serviceProvider.GetService(typeof(IVoucherCheckService)));
                Assert.IsNotNull(serviceProvider.GetService(typeof(ITokenVoucherGeneratorService)));
                Assert.IsNotNull(serviceProvider.GetService(typeof(IVoucherGeneratorService)));
                Assert.IsNotNull(serviceProvider.GetService(typeof(ITokenRepository)));
                Assert.IsNotNull(serviceProvider.GetService(typeof(IProductRepository)));
                Assert.IsNotNull(serviceProvider.GetService(typeof(IVendorCompanyRepository)));
                Assert.IsNotNull(serviceProvider.GetService(typeof(IEnterpriseRepository)));
            }
            
        }

        [TearDown]
        public void TearDown()
        {
            _factory?.Dispose();
        }
    }
}

