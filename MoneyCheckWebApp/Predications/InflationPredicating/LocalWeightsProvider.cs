using System;
using System.IO;
using System.Text.Json;
using MoneyCheckWebApp.Predications.InflationPredicating.NeuralNetwork;

namespace MoneyCheckWebApp.Predications.InflationPredicating
{
    public class LocalWeightsProvider : IWeightsProvider
    {
        private const string WeightsPath = "nn-weights.cache.json"; 
        
        public double[] ProvideWeights()
        {
            if (!File.Exists(WeightsPath))
            {
                throw new FileNotFoundException("Failed to load neural network weights becuase JSON not found");
            }

            var json = File.ReadAllText(WeightsPath);
            var weights = JsonSerializer.Deserialize<double[]>(json);

            if (weights == null)
            {
                throw new InvalidOperationException("Failed to load neural network weights becuase JSON has invalid structure");
            }

            return weights;
        }

        public void SetWeights(double[] weights)
        {
            var json = JsonSerializer.Serialize(weights);
            
            File.WriteAllText(WeightsPath, json);
        }
    }
}