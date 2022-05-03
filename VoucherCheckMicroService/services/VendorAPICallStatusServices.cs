using System;
using Beis.Htg.VendorSme.Database.Models;
using System.Text.Json;
using VoucherCheckService.domain.entities;
using VoucherCheckService.services.repositories;

namespace VoucherCheckMicroService.services.interfaces
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