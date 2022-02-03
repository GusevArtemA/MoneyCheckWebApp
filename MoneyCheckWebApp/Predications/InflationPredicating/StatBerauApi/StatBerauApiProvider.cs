using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MoneyCheckWebApp.Predications.InflationPredicating.StatBerauApi.ApiTypes;

namespace MoneyCheckWebApp.Predications.InflationPredicating.StatBerauApi
{
    public class StatBerauApiProvider
    {
        private readonly Uri _getInflationUri = new("https://www.statbureau.org/get-data-json?country=russia");
        
        public async Task<IEnumerable<Inflation>> GetInflationAsync()
        {
            using var http = new HttpClient();

            var response = await http.GetAsync(_getInflationUri);
            
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<IEnumerable<Inflation>>() ?? Array.Empty<Inflation>();
        }
    }
    
}