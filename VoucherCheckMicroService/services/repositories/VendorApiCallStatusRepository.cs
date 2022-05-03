using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;

namespace VoucherCheckService.services.repositories
{
    public class VendorApiCallStatusRepository: IVendorAPICallStatusRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public VendorApiCallStatusRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }

        public void LogRequestDetails(vendor_api_call_status vendorApiCallStatuses)
        {
            _context.vendor_api_call_statuses.Add(vendorApiCallStatuses);
            _context.SaveChanges();
        }
    }
}