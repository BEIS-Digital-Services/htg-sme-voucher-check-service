using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Beis.HelpToGrow.Voucher.Api.Check.Interfaces;
using Beis.HelpToGrow.Voucher.Api.Check.Controllers;
using Beis.HelpToGrow.Voucher.Api.Check.Domain.Entities;

namespace Beis.HelpToGrow.Voucher.Api.Check.Tests
{
    [TestFixture]
    public class VoucherControllerTests
    {
        private VoucherController _voucherController;
        private Mock<IVoucherCheckService> _voucherCheckService;
        private Mock<ILogger<VoucherController>> _logger;
        private Mock<IVendorAPICallStatusServices> _vendorApiCallStatusServices;

        [SetUp]
        public void Setup()
        {
            
            _voucherCheckService = new Mock<IVoucherCheckService>();
            _logger = new Mock<ILogger<VoucherController>>();
            _vendorApiCallStatusServices = new Mock<IVendorAPICallStatusServices>();
            _vendorApiCallStatusServices.Setup(x => x.CreateLogRequestDetails(It.IsAny<VoucherRequest>()))
                .Returns(new Beis.Htg.VendorSme.Database.Models.vendor_api_call_status { });
            _voucherController = new VoucherController(_logger.Object, _voucherCheckService.Object, _vendorApiCallStatusServices.Object);
        }
        
        [Test]
        public async Task CheckVoucherHappyPathTest ()
        {
            VoucherRequest voucherRequest = new VoucherRequest();
            
            voucherRequest.registration = "12345";
            voucherRequest.accessCode = "12345";
            voucherRequest.voucherCode = "IvMBLZ2PhUVkmJHpAxle0Q";
            
            _voucherCheckService.Setup(x => x.GetVoucherResponse(It.IsAny<VoucherRequest>())).ReturnsAsync(getVoucherResponse(voucherRequest));
            
            ActionResult<VoucherResponse> actionResult = await _voucherController.CheckVoucher(voucherRequest);

            VoucherResponse voucherResponse = (VoucherResponse) ((OkObjectResult) actionResult.Result).Value;
            
            Assert.AreEqual(voucherRequest.voucherCode, voucherResponse.voucherCode);
            Assert.AreEqual(0, voucherResponse.errorCode);
            Assert.AreEqual("GHT23RTDWER", voucherResponse.authorisationCode);
            Assert.AreEqual("ABC corporation", voucherResponse.vendor);
            Assert.AreEqual("GHU1234", voucherResponse.productSku);
            Assert.AreEqual("Mr. Joe Blogs", voucherResponse.purchaserName);
            Assert.AreEqual("Buyer limited", voucherResponse.licenceTo);
            Assert.AreEqual("abc@my-company.com", voucherResponse.smeEmail);
            Assert.AreEqual(4999, voucherResponse.maxDiscountAllowed);
        }
       
        [Test]
        public async Task CheckVoucherNegativePathTest () 
        {
            VoucherRequest voucherRequest = new VoucherRequest();
            
            voucherRequest.registration = "12345";
            voucherRequest.accessCode = "12345";
            voucherRequest.voucherCode = "IvMBLZ2PhUVkmJHpAxle0Qcc";
            
            _voucherCheckService.Setup(x => x.GetVoucherResponse(It.IsAny<VoucherRequest>())).Throws(new Exception("my exception"));
            
            ActionResult<VoucherResponse> actionResult = await _voucherController.CheckVoucher(voucherRequest);

            VoucherResponse voucherResponse = (VoucherResponse) ((ObjectResult) actionResult.Result).Value;
            
            Assert.AreEqual(voucherRequest.voucherCode, voucherResponse.voucherCode);
            Assert.AreEqual("ERROR", voucherResponse.status);
            Assert.AreEqual(10, voucherResponse.errorCode);
            Assert.AreEqual("Unknown token my exception", voucherResponse.message);
        }

        [Test]
        public async Task CheckVoucherReturnsStatus400PathTest()
        {
            VoucherRequest voucherRequest = new VoucherRequest();

            voucherRequest.registration = "12345";
            voucherRequest.accessCode = "12345";
            voucherRequest.voucherCode = "IvMBLZ2PhUVkmJHpAxle0Qcc";

            var expectedResponse = new VoucherResponse 
            { 
                errorCode = 400,
                voucherCode = "IvMBLZ2PhUVkmJHpAxle0Qcc",
                status = "ERROR"
            };

            _voucherCheckService.Setup(x => x.GetVoucherResponse(It.IsAny<VoucherRequest>())).ReturnsAsync(expectedResponse);

            ActionResult<VoucherResponse> actionResult = await _voucherController.CheckVoucher(voucherRequest);

            VoucherResponse voucherResponse = (VoucherResponse)((ObjectResult)actionResult.Result).Value;

            Assert.AreEqual(voucherRequest.voucherCode, voucherResponse.voucherCode);
            Assert.AreEqual("ERROR", voucherResponse.status);
            Assert.AreEqual(400, voucherResponse.errorCode);
        }

        private VoucherResponse getVoucherResponse(VoucherRequest voucherRequest)
        {
            return new VoucherResponse()
            {
                voucherCode = voucherRequest.voucherCode,
                errorCode = 0,
                authorisationCode = "GHT23RTDWER",
                vendor = "ABC corporation",
                productSku = "GHU1234",
                purchaserName = "Mr. Joe Blogs",
                licenceTo = "Buyer limited",
                smeEmail = "abc@my-company.com",
                maxDiscountAllowed = 4999
            }; }
        
    }
}