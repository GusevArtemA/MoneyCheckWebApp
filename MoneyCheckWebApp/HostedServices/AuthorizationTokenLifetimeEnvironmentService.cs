using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.HostedServices
{
    public class AuthorizationTokenLifetimeEnvironmentService : IHostedService, IDisposable
    {
        private readonly ILogger<AuthorizationTokenLifetimeEnvironmentService> _logger;
        private readonly MoneyCheckDbContext _context;

        private Timer? _timer;
        
        public AuthorizationTokenLifetimeEnvironmentService(ILogger<AuthorizationTokenLifetimeEnvironmentService> logger, MoneyCheckDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Token ecosystem started");

            _timer = new Timer(CheckTokens, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            
            return Task.CompletedTask;
        }

        private void CheckTokens(object? state)
        {
            var now = DateTime.Now;
            
            _context.UserAuthTokens.RemoveRange(
                _context.UserAuthTokens.Where(x => x.ExpiresAt < now));

            _context.SaveChanges();
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Token ecosystem stopped");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}