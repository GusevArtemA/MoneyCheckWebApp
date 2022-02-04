using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.Predications.InflationPredicating;

namespace MoneyCheckWebApp.Controllers
{
    [ApiController]
    [Route("/api/inflation-predication")]
    public class InfaltionPredicationController : ControllerBase
    {
        private readonly InflationPredicationProcessor _inflationPredicationProcessor;

        public InfaltionPredicationController(InflationPredicationProcessor inflationPredicationProcessor)
        {
            _inflationPredicationProcessor = inflationPredicationProcessor;
        }
        
        [HttpGet]
        [Route("predict")]
        public async Task<IActionResult> Predict(double priceNow, int index = 1)
        {
            var predication = await _inflationPredicationProcessor.PredicateAsync(index);

            return Ok(Math.Round(predication* priceNow));
        }
    }
}