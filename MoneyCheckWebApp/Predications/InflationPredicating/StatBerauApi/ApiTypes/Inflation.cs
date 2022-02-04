using System;

namespace MoneyCheckWebApp.Predications.InflationPredicating.StatBerauApi.ApiTypes
{
    public class Inflation
    {
        public double InflationRate { get; init; }
        public DateTime MonthFormatted { get; init; }
    }
}