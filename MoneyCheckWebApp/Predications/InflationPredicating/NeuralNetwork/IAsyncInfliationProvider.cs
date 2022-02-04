using System.Threading.Tasks;

namespace MoneyCheckWebApp.Predications.InflationPredicating.NeuralNetwork
{
    public interface IAsyncInfliationProvider
    {
        Task<double[]> ProvideInflationArrayAsync();
    }
}