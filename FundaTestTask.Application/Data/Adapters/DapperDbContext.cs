using FundaTestTask.Application.Data.Ports;
using Npgsql;

namespace FundaTestTask.Application.Data.Adapters
{
    public class DapperDbContext : IDbContext
    {
        public DapperDbContext(IConnectionOptions connectionOptions)
        {            
            ConnectionString = connectionOptions.PostgresConnectionString ?? throw new ArgumentNullException("Connection string 'PostgresConnection' not found.");
            Up();
        }

        public string ConnectionString { get; }

        private void Up()
        {
            using var dbConnection = new NpgsqlConnection(ConnectionString);

            dbConnection.Open();

            var sql = @"
                CREATE TABLE IF NOT EXISTS public.""Properties"" (
                    ""Id"" UUID PRIMARY KEY NOT NULL,
                    ""EstateAgentId"" INT NOT NULL,
                    ""EstateAgentName"" VARCHAR(255) NOT NULL,
                    ""HasGardenMention"" BOOLEAN NOT NULL
                );";

            var command = new NpgsqlCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }
    }
}
