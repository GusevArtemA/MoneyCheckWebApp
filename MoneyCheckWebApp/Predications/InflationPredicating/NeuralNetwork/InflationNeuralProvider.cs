#nullable disable

using System;
using System.Threading.Tasks;

namespace MoneyCheckWebApp.Predications.InflationPredicating.NeuralNetwork
{
    public class InflationNeuralProvider
    {
        private readonly InflationNeuralNetworkProviderConfiguration _config;
        private readonly InflationNeuralNetworkEngine _nn;
        
        public InflationNeuralProvider(InflationNeuralNetworkProviderConfiguration config)
        {
            _config = config;
            _nn = new InflationNeuralNetworkEngine(_config.NumInputsNeurons,
                _config.NumHiddenNeurons,
                _config.NumOutputNeurons);
        }

        public double[] TrainNeuralNetwork(InflationNeuralNetworkTrainingConfiguration trainingConfiguration)
        {
            var formattedData = FormatData(trainingConfiguration.InflationRates);

            return _nn.Train(formattedData, trainingConfiguration.MaxEpochs, trainingConfiguration.LearnRate);
        }

        private double[][] FormatData(double[] dataToFormat)
        {
            var matrix = new double[dataToFormat.Length - _config.NumInputsNeurons][];
            
            for (var i = 0; i < dataToFormat.Length - _config.NumInputsNeurons; i++)
            {
                matrix[i] = new double[_config.NumInputsNeurons + 1];

                for (var j = 0; j < _config.NumInputsNeurons + 1; j++)
                {
                    matrix[i][j] = dataToFormat[i + j];
                }
            }

            return matrix;
        }
        
        public async Task<double> PredictInflationAsync(IWeightsProvider weightsProvider, IAsyncInfliationProvider asyncInfliationProvider, int index)
        {
            var weights = weightsProvider.ProvideWeights();
            var inflationIndexes = await asyncInfliationProvider.ProvideInflationArrayAsync();
            var mult = 1d;

            double[] inflationIndexesBuffer = new double[_config.NumInputsNeurons];
            
            Array.Copy(inflationIndexes, inflationIndexes, _config.NumInputsNeurons);

            _nn.SetWeights(weights);
            
            for (int i = 0; i < index; i++)
            {
                var outputs = _nn.ComputeOutputs(inflationIndexesBuffer);
                
                Array.Copy(inflationIndexesBuffer, 1, inflationIndexesBuffer, 0, inflationIndexesBuffer.Length - 1);
                inflationIndexesBuffer[_config.NumInputsNeurons - 1] = outputs[0];
                mult *= outputs[0] / 100 + 1;
            }

            return mult;
        }
    }

    public class InflationNeuralNetworkProviderConfiguration
    {
        /// <summary>
        /// Количество входных нейронов
        /// </summary>
        public int NumInputsNeurons { get; init; }
        
        /// <summary>
        /// Количество скрытых нейронов
        /// </summary>
        public int NumHiddenNeurons { get; init; }
        
        /// <summary>
        /// Количество выходных нейронов
        /// </summary>
        public int NumOutputNeurons { get; init; }
    }

    public class InflationNeuralNetworkTrainingConfiguration
    {
        /// <summary>
        /// Скорость обучения
        /// </summary>
        public double LearnRate { get; init; }
        
        /// <summary>
        /// Максимальное количество эпох обучения
        /// </summary>
        public int MaxEpochs { get; set; }
        
        /// <summary>
        /// Показатели инфляций
        /// </summary>
        public double[] InflationRates { get; set; }
    }
}