using Beis.Htg.VendorSme.Database.Models;
using Beis.HelpToGrow.Voucher.Api.Check.Domain.Entities;

namespace Beis.HelpToGrow.Voucher.Api.Check.Interfaces
{
    public interface IVendorAPICallStatusServices
    {
        vendor_api_call_status  CreateLogRequestDetails(VoucherRequest voucherRequest);
        void LogRequestDetails(vendor_api_call_status vendorApiCallStatuses);
    }
}