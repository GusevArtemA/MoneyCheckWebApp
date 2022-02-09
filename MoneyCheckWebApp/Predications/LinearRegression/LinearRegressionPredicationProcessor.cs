using System;
using System.Linq;

namespace MoneyCheckWebApp.Predications.LinearRegression
{
    public class LinearRegressionPredicationProcessor
    {
        private readonly decimal[] _array;
        private decimal tan;
        private decimal bParam;
        
        private LinearRegressionPredicationProcessor(IDecimalArrayProvider provider)
        {
            _array = provider.ProvideArray();
        }

        private void Initialize()
        {
            if (_array.Length == 0)
            {
                throw new InvalidOperationException("Empty array provider");
            }
            
            var averageY = _array.Average();
            var averageX = (1 + _array.Length) / 2;

            decimal highSum = 0;
            decimal lowSum = 0;
            
            decimal CalculateY(int i) => _array[i] - averageY;
            decimal CalculateX(int i) => i - averageX;
            
            for (int i = 0; i < _array.Length; i++)
            {
                var x = CalculateX(i);
                
                highSum += x * CalculateY(i);
                lowSum += x * x;
            }

            tan = highSum / lowSum;
            bParam = averageY - tan * averageX;
        }
        
        /// <summary>
        /// Производит расчет на следующий временной промежуток с помощью модели линейной регрессии
        /// </summary>
        public decimal PredicateNext(int index = 1) => Math.Round(tan * (_array.Length + index) + bParam);

        public static LinearRegressionPredicationProcessor Initialize(IDecimalArrayProvider arrayProvider)
        {
            var proc = new LinearRegressionPredicationProcessor(arrayProvider);
            
            proc.Initialize();

            return proc;
        }
    }
}