using Beis.Htg.VendorSme.Database.Models;

namespace Beis.HelpToGrow.Voucher.Api.Check.Interfaces
{
    public interface IVendorAPICallStatusRepository
    {
        void LogRequestDetails(vendor_api_call_status vendorApiCallStatuses);
    }
}