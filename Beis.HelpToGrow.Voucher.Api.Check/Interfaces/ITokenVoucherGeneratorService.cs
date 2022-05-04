using System.Threading.Tasks;
using Beis.HelpToGrow.Voucher.Api.Check.Domain.Entities;

namespace Beis.HelpToGrow.Voucher.Api.Check.Interfaces
{
    public interface ITokenVoucherGeneratorService
    {
        Task<VoucherGenerationResponse> GenerateVoucher(VoucherGenerationRequest voucherGenerationRequest);
    }
}