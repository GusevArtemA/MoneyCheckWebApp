using System;
using System.IO;

namespace MoneyCheckWebApp.Predications.InflationPredicating
{
    public static class LocalWeightsLifeTimeService
    {
        private const string WeightsPath = "nn-weights.json";
        
        public static bool CheckIfUpdateRequired()
        {
            if (!File.Exists(WeightsPath))
            {
                return true;
            }

            var fileInfo = new FileInfo(WeightsPath);

            return DateTime.UtcNow - fileInfo.LastWriteTimeUtc > TimeSpan.FromDays(31);
        }
    }
}