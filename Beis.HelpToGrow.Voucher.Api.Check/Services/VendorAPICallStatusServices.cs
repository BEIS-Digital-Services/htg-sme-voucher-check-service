using System;
using Beis.Htg.VendorSme.Database.Models;
using System.Text.Json;
using Beis.HelpToGrow.Voucher.Api.Check.Domain.Entities;
using Beis.HelpToGrow.Voucher.Api.Check.Services.Repositories;
using Beis.HelpToGrow.Voucher.Api.Check.Interfaces;

namespace Beis.HelpToGrow.Voucher.Api.Check.Services
{
    public class VendorAPICallStatusServices: IVendorAPICallStatusServices
    {
        private IVendorAPICallStatusRepository _vendorApiCallStatusRepository;
        
        public VendorAPICallStatusServices(IVendorAPICallStatusRepository vendorApiCallStatusRepository)
        {
            _vendorApiCallStatusRepository = vendorApiCallStatusRepository;
        }

        public vendor_api_call_status CreateLogRequestDetails(VoucherRequest voucherRequest)
        {
            var apiCallStatus = new vendor_api_call_status
            {
                vendor_id = new[] {Convert.ToInt64(voucherRequest.registration.Substring(1, voucherRequest.registration.Length -1))},
                api_called = "voucherCheck",
                call_datetime = DateTime.Now,
                request = JsonSerializer.Serialize(voucherRequest)
            };

            return apiCallStatus;
        }

        public void LogRequestDetails(vendor_api_call_status vendorApiCallStatuses)
        {
            _vendorApiCallStatusRepository.LogRequestDetails(vendorApiCallStatuses);
        }
    }
}