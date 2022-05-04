using System;
using System.Text.Json;
using Beis.Htg.VendorSme.Database.Models;
using Moq;
using NUnit.Framework;
using Beis.HelpToGrow.Voucher.Api.Check.Interfaces;
using Beis.HelpToGrow.Voucher.Api.Check.Domain.Entities;
using Beis.HelpToGrow.Voucher.Api.Check.Services.Repositories;
using Beis.HelpToGrow.Voucher.Api.Check.Services;

namespace Beis.HelpToGrow.Voucher.Api.Check.Tests
{
    [TestFixture]
    
    public class VendorAPICallStatusServicesTests
    {
        private VendorAPICallStatusServices _vendorApiCallStatusServices;
        private Mock<IVendorAPICallStatusRepository> _vendorApiCallStatusRepository;

        [SetUp]

        public void Setup()
        {
            _vendorApiCallStatusRepository = new Mock<IVendorAPICallStatusRepository>();
            _vendorApiCallStatusServices = new VendorAPICallStatusServices(_vendorApiCallStatusRepository.Object);
        }

        [Test]

        public void VendorAPICallStatusServicesCreateLogRequestDetailsHappyTests()
        {
            var voucherRequest = setupTestData();
            var vendor_api_call_status = _vendorApiCallStatusServices.CreateLogRequestDetails(voucherRequest);
            
            Assert.AreEqual("voucherCheck", vendor_api_call_status.api_called);
            Assert.NotNull(vendor_api_call_status.vendor_id);
            Assert.NotNull(vendor_api_call_status.request);
            Assert.NotNull(vendor_api_call_status.call_datetime);

        }

        [Test]

        public void VendorAPICallStatusServicesLogRequestDetailsHappyTests()
        {
            var vendorAPICallStatus = logRequestDetailsSetupTestData();
            
            _vendorApiCallStatusServices.LogRequestDetails(vendorAPICallStatus);
            
            _vendorApiCallStatusRepository.Verify(v => v.LogRequestDetails(It.IsAny<vendor_api_call_status>()));
        }

        private VoucherRequest setupTestData()
        {
            VoucherRequest voucherRequest = new VoucherRequest()
            {
                registration = "12345",
                accessCode = "12345",
                voucherCode = "IvMBLZ2PhUVkmJHpAxle0Q"
            };

            return voucherRequest;
        }
        
        private vendor_api_call_status logRequestDetailsSetupTestData()
        {
            var voucherRequest = new VoucherRequest();
            voucherRequest = setupTestData();

            var vendorApiCallStatus = new vendor_api_call_status()
            {
                call_id = 12345,
                vendor_id = new[] {Convert.ToInt64(voucherRequest.registration.Substring(1, voucherRequest.registration.Length -1))},
                api_called = "VoucherCheck",
                call_datetime = DateTime.Now,
                request = JsonSerializer.Serialize(voucherRequest)
            };

            _vendorApiCallStatusRepository.Setup(x => x.LogRequestDetails(It.IsAny<vendor_api_call_status>()));
            
            return vendorApiCallStatus;

        }
        
        
    }
    
    
}