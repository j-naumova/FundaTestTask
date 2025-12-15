using FundaTestTask.Application.Data.Ports;

namespace FundaTestTask.ConsoleClient.ConsoleSpecific
{
    internal class ConnectionOptions : IConnectionOptions
    {
        public string PostgresConnectionString => ConfigHelper.GetProperty(nameof(PostgresConnectionString));
    }
}
