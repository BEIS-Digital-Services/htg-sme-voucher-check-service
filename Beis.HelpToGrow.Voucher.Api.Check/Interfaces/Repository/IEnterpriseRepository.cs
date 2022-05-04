using System.Threading.Tasks;
using Beis.Htg.VendorSme.Database.Models;

namespace Beis.HelpToGrow.Voucher.Api.Check.Interfaces
{
    public interface IEnterpriseRepository
    {
        Task<enterprise> GetEnterprise(long enterpriseId);
    }
}