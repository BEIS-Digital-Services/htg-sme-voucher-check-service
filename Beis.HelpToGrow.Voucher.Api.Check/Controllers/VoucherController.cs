using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Beis.Htg.VendorSme.Database.Models;
using Beis.HelpToGrow.Voucher.Api.Check.Interfaces;
using Beis.HelpToGrow.Voucher.Api.Check.Domain.Entities;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Voucher.Api.Check.Controllers
{
    [ApiController]
    [Route("api/vouchercheck")]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherCheckService _voucherCheckService;
        private ILogger<VoucherController> _logger;
        private IVendorAPICallStatusServices _vendorApiCallStatusServices;

        public VoucherController(ILogger<VoucherController> logger, IVoucherCheckService voucherCheckService, IVendorAPICallStatusServices vendorApiCallStatusServices)
        {
            _voucherCheckService = voucherCheckService;
            _logger = logger;
            _vendorApiCallStatusServices = vendorApiCallStatusServices;
        }

        /// <summary>
        /// Voucher check endpoint
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/vouchercheck
        ///     {
        ///        "registration": 12345,
        ///        "accessCode": "12345",
        ///        "voucherCode": "sH9ftM1rvm6N635qFVNdhg"
        ///     }
        ///
        /// </remarks>
        ///
        /// 
        [HttpPost]
        [ProducesResponseType(typeof(VoucherResponse), 200)]
        [ProducesResponseType(typeof(VoucherResponse), 400)]
        [ProducesResponseType(typeof(VoucherResponse), 500)]
        public async Task<ActionResult<VoucherResponse>> CheckVoucher([FromBody] VoucherRequest voucherRequest)
        {
            _logger.LogInformation("VoucherCheckControllerRequest: {@VoucherRequest}", JsonSerializer.Serialize(voucherRequest));
            
            VoucherResponse voucherResponse;
            try
            {
                voucherResponse = await _voucherCheckService.GetVoucherResponse(voucherRequest);
                if (voucherResponse.errorCode == 0)
                {
                    _logger.LogInformation("VoucherCheckControllerResponse: {@voucherResponse}", JsonSerializer.Serialize(voucherResponse));
                    return Ok(voucherResponse);
                }
                _logger.LogInformation("VoucherCheckControllerResponse: {@voucherResponse}", JsonSerializer.Serialize(voucherResponse));
                return StatusCode(400, voucherResponse);                
            }
            catch (Exception e)
            {
                 voucherResponse = new VoucherResponse
                {
                    status = "ERROR",
                    errorCode = 10,
                    message = "Unknown token " + e.Message,
                    voucherCode = voucherRequest.voucherCode
                };

                _logger.LogInformation("VoucherCheckControllerResponse: {@voucherResponse}", JsonSerializer.Serialize(voucherResponse));
                var vendor_api_call_status = _vendorApiCallStatusServices.CreateLogRequestDetails(voucherRequest);
                vendor_api_call_status.error_code = "500";
                LogAPiCallStatus(vendor_api_call_status, voucherResponse);

                return StatusCode(500, voucherResponse);
            }                       
        }

        private void LogAPiCallStatus(vendor_api_call_status vendor_api_call_status, VoucherResponse voucherResponse)
        {
            vendor_api_call_status.result = JsonSerializer.Serialize(voucherResponse);
            _vendorApiCallStatusServices.LogRequestDetails(vendor_api_call_status);
        }
    }
}