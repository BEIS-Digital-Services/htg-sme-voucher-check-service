using System.Threading.Tasks;
using VoucherCheckService.domain.entities;

namespace VoucherCheckService.interfaces
{
    public interface ITokenVoucherGeneratorService
    {
        Task<VoucherGenerationResponse> GenerateVoucher(VoucherGenerationRequest voucherGenerationRequest);
    }
}