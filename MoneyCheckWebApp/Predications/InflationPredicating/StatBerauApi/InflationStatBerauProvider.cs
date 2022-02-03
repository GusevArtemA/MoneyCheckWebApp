using System;
using System.Linq;
using System.Threading.Tasks;
using MoneyCheckWebApp.Extensions;
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

            return arrayedInfaltions.Select(x => x.InflationRate).ToArray().GetLastElements(120);
        }
    }
}