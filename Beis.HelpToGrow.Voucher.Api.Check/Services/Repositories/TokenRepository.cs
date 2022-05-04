using System.Linq;
using System.Threading.Tasks;
using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;
using Beis.HelpToGrow.Voucher.Api.Check.Interfaces;

namespace Beis.HelpToGrow.Voucher.Api.Check.Services.Repositories
{
    public class TokenRepository: ITokenRepository
    {
        private readonly HtgVendorSmeDbContext _context;

        public TokenRepository(HtgVendorSmeDbContext context)
        {
            _context = context;
        }
        
        public async Task AddToken(token token)
        {
            await _context.tokens.AddAsync(token);
            var result = await _context.SaveChangesAsync();
        }

        public token GetToken(string tokenCode)
        {
            var token = _context.tokens.SingleOrDefault(t => t.token_code == tokenCode);           
            return token;
        }
        
        public async Task UpdateToken(token token)
        {
            _context.tokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}