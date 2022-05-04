using Beis.Htg.VendorSme.Database.Models;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Voucher.Api.Check.Interfaces
{
    public interface IVoucherGeneratorService
    {
        public Task<string> GenerateVoucher(vendor_company vendorCompany, enterprise enterprise, product product);
        public string GenerateSetCode(int length);
    }
}