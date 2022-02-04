namespace MoneyCheckWebApp.Predications.InflationPredicating.NeuralNetwork
{
    public interface IWeightsProvider
    {
        double[] ProvideWeights();
    }
}