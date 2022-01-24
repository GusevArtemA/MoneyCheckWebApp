using System.Linq;

namespace MoneyCheckWebApp.PricePredicating
{
    public class PredicationProcessor
    {
        private readonly decimal[] _array;
        
        public PredicationProcessor(IDecimalArrayProvider provider)
        {
            _array = provider.ProvideArray();
        }
        
        /// <summary>
        /// Производит расчет на следующий месяц с помощью модели линейной регрессии
        /// </summary>
        public decimal PredicateNext()
        {
            if (_array.Length == 0)
            {
                return 0m;
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

            var tan = highSum / lowSum;
            var bParam = averageY - tan * averageX;

            return tan * (_array.Length + 1) + bParam;
        }
    }
}