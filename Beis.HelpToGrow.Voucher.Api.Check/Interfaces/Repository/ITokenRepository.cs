using System.Threading.Tasks;
using Beis.Htg.VendorSme.Database.Models;

namespace Beis.HelpToGrow.Voucher.Api.Check.Interfaces
{
    public interface ITokenRepository
    {
        Task AddToken(token token);
        token GetToken(string tokenCode);
        Task UpdateToken(token token);
        
    }
}