using System.Threading.Tasks;
using Beis.HelpToGrow.Voucher.Api.Check.Domain.Entities;

namespace Beis.HelpToGrow.Voucher.Api.Check.Interfaces
{
    public interface IVoucherCheckService
    {
        public Task<VoucherResponse> GetVoucherResponse(VoucherRequest voucherRequest);
    }
}