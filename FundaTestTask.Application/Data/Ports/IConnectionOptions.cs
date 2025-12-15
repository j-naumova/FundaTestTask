namespace FundaTestTask.Application.Data.Ports
{
    public interface IConnectionOptions
    {
        string PostgresConnectionString { get; }
    }
}
