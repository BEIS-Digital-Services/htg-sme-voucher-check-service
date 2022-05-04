using Beis.HelpToGrow.Voucher.Api.Check.Interfaces;
using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;

namespace Beis.HelpToGrow.Voucher.Api.Check.Services.Repositories
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