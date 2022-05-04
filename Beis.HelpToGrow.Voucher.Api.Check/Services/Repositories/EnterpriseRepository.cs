using System.Linq;
using System.Threading.Tasks;
using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;
using Microsoft.EntityFrameworkCore;
using Beis.HelpToGrow.Voucher.Api.Check.Interfaces;

namespace Beis.HelpToGrow.Voucher.Api.Check.Services.Repositories
{
    public class EnterpriseRepository: IEnterpriseRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public EnterpriseRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }
        public async Task<enterprise> GetEnterprise(long enterpriseId)
        {
            return await _context.enterprises.FirstOrDefaultAsync(t => t.enterprise_id == enterpriseId);
        }
    }
}