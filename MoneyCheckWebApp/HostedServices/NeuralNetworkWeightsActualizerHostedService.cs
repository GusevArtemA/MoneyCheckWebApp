using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MoneyCheckWebApp.Predications.InflationPredicating;

namespace MoneyCheckWebApp.HostedServices
{
    public class NeuralNetworkWeightsActualizerHostedService : BackgroundService
    {
        private readonly InflationPredicationProcessor _processor;

        public NeuralNetworkWeightsActualizerHostedService(InflationPredicationProcessor processor)
        {
            _processor = processor;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _processor.TrainIfRequiredAsync();
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}