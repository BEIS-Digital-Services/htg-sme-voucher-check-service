using Beis.Htg.VendorSme.Database.Models;

namespace VoucherCheckService.services.repositories
{
    public interface IVendorAPICallStatusRepository
    {
        void LogRequestDetails(vendor_api_call_status vendorApiCallStatuses);
    }
}