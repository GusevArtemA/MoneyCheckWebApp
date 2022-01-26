using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.HostedServices
{
    public class AuthorizationTokenLifetimeEnvironmentService : BackgroundService
    {
        private readonly ILogger<AuthorizationTokenLifetimeEnvironmentService> _logger;
        private readonly MoneyCheckDbContext _context;
        
        public AuthorizationTokenLifetimeEnvironmentService(ILogger<AuthorizationTokenLifetimeEnvironmentService> logger, IServiceScopeFactory factory)
        {
            _logger = logger;
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<MoneyCheckDbContext>();
        }

        private void CheckTokens(MoneyCheckDbContext dbContext)
        {
            var now = DateTime.Now;

            dbContext.UserAuthTokens.RemoveRange(
                dbContext.UserAuthTokens.AsEnumerable().Where(x => now - x.ExpiresAt > TimeSpan.Zero));

            dbContext.SaveChanges();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Token ecosystem started");
            while (!stoppingToken.IsCancellationRequested)
            {
                CheckTokens(_context);
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
            _logger.LogInformation("Token ecosystem ended");
        }
    }
}