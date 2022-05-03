using System.Threading.Tasks;
using VoucherCheckService.domain.entities;

namespace VoucherCheckService.interfaces
{
    public interface IVoucherCheckService
    {
        public Task<VoucherResponse> GetVoucherResponse(VoucherRequest voucherRequest);
    }
}