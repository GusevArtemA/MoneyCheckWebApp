using System;
using System.Threading.Tasks;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.Services
{
    public class AuthorizationService
    {
        private readonly MoneyCheckDbContext _context;

        public AuthorizationService(MoneyCheckDbContext context)
        {
            _context = context;
        }

        public async Task<UserAuthToken> GenerateTokenAsync(User user, int tokenLifeTime)
        {
            var token = new UserAuthToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.Now + TimeSpan.FromSeconds(tokenLifeTime)
            };

            await _context.UserAuthTokens.AddAsync(token);

            token.User = user;

            await _context.SaveChangesAsync();

            return token;
        }
    }
}