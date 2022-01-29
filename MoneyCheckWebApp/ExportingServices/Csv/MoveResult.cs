namespace MoneyCheckWebApp.ExportingServices.Csv
{
    public class MoveResult
    {
        public bool End { get; init; }
        public string Result { get; init; }

        public static MoveResult EndResult() => new MoveResult()
        {
            End = true
        };

        public static MoveResult WithResult(string res) => new()
        {
            End = false,
            Result = res
        };
    }
}