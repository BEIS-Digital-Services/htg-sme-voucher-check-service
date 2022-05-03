using Beis.Htg.VendorSme.Database.Models;
using VoucherCheckService.domain.entities;

namespace VoucherCheckMicroService.services.interfaces
{
    public interface IVendorAPICallStatusServices
    {
        vendor_api_call_status  CreateLogRequestDetails(VoucherRequest voucherRequest);
        void LogRequestDetails(vendor_api_call_status vendorApiCallStatuses);
    }
}