using System;
using System.IO;
using System.Linq;
using System.Text.Json;
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
            const string statBerauCachePath = "stat-berau-cache.cache.json";
            var cacheFileInfo = new FileInfo(statBerauCachePath);
                
            if (cacheFileInfo.Exists && DateTime.UtcNow - cacheFileInfo.LastAccessTimeUtc <= TimeSpan.FromDays(3))
            {
                var fileText = await File.ReadAllTextAsync(statBerauCachePath);

                if (!string.IsNullOrEmpty(fileText))
                {
                    var deserializedArray = JsonSerializer.Deserialize<double[]>(fileText);

                    if (deserializedArray != null)
                    {
                        return deserializedArray.Reverse().ToArray();
                    }
                }
            }
            
            var apiProvider = new StatBerauApiProvider();
            var infl = await apiProvider.GetInflationAsync();

            var arrayedInfaltions = infl as Inflation[] ?? infl.ToArray();
            if (!arrayedInfaltions.Any())
            {
                throw new InvalidOperationException("Failed to fetch non-empty array from statberau.com");
            }

            var doubleArrayed = arrayedInfaltions.Select(x => x.InflationRate).ToArray().GetLastElements(120);
            
            await File.WriteAllTextAsync(statBerauCachePath, JsonSerializer.Serialize(doubleArrayed));

            return doubleArrayed.Reverse().ToArray();
        }
    }
}