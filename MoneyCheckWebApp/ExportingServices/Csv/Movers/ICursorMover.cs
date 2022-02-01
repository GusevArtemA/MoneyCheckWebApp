namespace MoneyCheckWebApp.ExportingServices.Csv.Movers
{
    public interface ICursorMover
    {
        MoveResult MoveNext();
    }
}