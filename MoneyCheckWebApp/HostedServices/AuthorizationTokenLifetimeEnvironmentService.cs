using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.HostedServices
{
    public class AuthorizationTokenLifetimeEnvironmentService : BackgroundService
    {
        private readonly ILogger<AuthorizationTokenLifetimeEnvironmentService> _logger;

        public AuthorizationTokenLifetimeEnvironmentService(ILogger<AuthorizationTokenLifetimeEnvironmentService> logger)
        {
            _logger = logger;
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
            await using MoneyCheckDbContext dbContext = new();
            
            _logger.LogInformation("Token ecosystem started");
            while (!stoppingToken.IsCancellationRequested)
            {
                CheckTokens(dbContext);
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
            _logger.LogInformation("Token ecosystem ended");
        }
    }
}