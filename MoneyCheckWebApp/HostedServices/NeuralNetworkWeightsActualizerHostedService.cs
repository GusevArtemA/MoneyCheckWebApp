using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoneyCheckWebApp.Predications.InflationPredicating;

namespace MoneyCheckWebApp.HostedServices
{
    public class NeuralNetworkWeightsActualizerHostedService : BackgroundService
    {
        private readonly InflationPredicationProcessor _processor;
        private readonly ILogger _logger;

        public NeuralNetworkWeightsActualizerHostedService(ILogger<NeuralNetworkWeightsActualizerHostedService> logger, IServiceScopeFactory scopeFactory)
        {
            _processor = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<InflationPredicationProcessor>();
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Weights actualizer started");
            while (!stoppingToken.IsCancellationRequested)
            {
                await _processor.TrainIfRequiredAsync();
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}