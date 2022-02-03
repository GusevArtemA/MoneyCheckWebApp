using System;
using System.Linq;
using System.Threading.Tasks;
using MoneyCheckWebApp.Predications.InflationPredicating.NeuralNetwork;
using MoneyCheckWebApp.Predications.InflationPredicating.StatBerauApi.ApiTypes;

namespace MoneyCheckWebApp.Predications.InflationPredicating.StatBerauApi
{
    public class InflationStatBerauProvider : IAsyncInfliationProvider
    {
        public async Task<double[]> ProvideInflationArrayAsync()
        {
            var apiProvider = new StatBerauApiProvider();
            
            var infl = await apiProvider.GetInflationAsync();

            var arrayedInfaltions = infl as Inflation[] ?? infl.ToArray();
            if (!arrayedInfaltions.Any())
            {
                throw new InvalidOperationException("Failed to fetch non-empty array from statberau.com");
            }

            var selectedIndexes = arrayedInfaltions.Select(x => x.InflationRate).ToList();

            if (selectedIndexes.Count > 120)
            {
                selectedIndexes.RemoveRange(120, selectedIndexes.Count - 120);
            }

            return selectedIndexes.ToArray();
        }
    }
}