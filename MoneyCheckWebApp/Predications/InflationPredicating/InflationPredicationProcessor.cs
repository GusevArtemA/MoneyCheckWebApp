using System.Threading.Tasks;
using MoneyCheckWebApp.Predications.InflationPredicating.NeuralNetwork;
using MoneyCheckWebApp.Predications.InflationPredicating.StatBerauApi;

namespace MoneyCheckWebApp.Predications.InflationPredicating
{
    public class InflationPredicationProcessor
    {
        private readonly InflationNeuralProvider _nnProvider = new(
            new InflationNeuralNetworkProviderConfiguration()
            {
                NumHiddenNeurons = 12,
                NumInputsNeurons = 6,
                NumOutputNeurons = 1
            });

        private LocalWeightsProvider _localWeightsProvider = new();
        private InflationStatBerauProvider _inflationStatBerauProvider = new();
        
        public bool NeedUpdate => LocalWeightsLifeTimeService.CheckIfUpdateRequired();

        /// <summary>
        /// Выполняет предсказание на указанное количество месяцев вперед
        /// </summary>
        /// <param name="index">Количество месяцев для предсказания (рекомендуется до указывать до 12 месяцев)</param>
        public async Task<double> PredicateAsync(int index)
        {
            return await _nnProvider.PredictInflationAsync(_localWeightsProvider, _inflationStatBerauProvider, index);
        }
        
        public async Task TrainAsync()
        {
            var trainConfig = await GenerateTrainingConfigAsync();

            var weights = _nnProvider.TrainNeuralNetwork(trainConfig);
            
            _localWeightsProvider.SetWeights(weights);
        }

        public async Task TrainIfRequiredAsync()
        {
            if (NeedUpdate)
            {
                await TrainAsync();
            }
        }
        
        private async Task<InflationNeuralNetworkTrainingConfiguration> GenerateTrainingConfigAsync() =>  new()
        {
            InflationRates = await _inflationStatBerauProvider.ProvideInflationArrayAsync(),
            LearnRate = 0.01,
            MaxEpochs = 10000
        };
    }
}