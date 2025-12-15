using Dapper;
using FundaTestTask.Application.Common.Models;
using FundaTestTask.Application.Data.Ports;

namespace FundaTestTask.Application.Data.Adapters
{
    public class PropertyInformationRepository(IDbContext dbContext) : IPropertyInformationRepository
    {
        public async Task<List<ResultRow>> GetReport(bool filterByHasGarden, CancellationToken token = default)
        {
            var sql = @"SELECT COUNT(""Id"") AS ""TotalProperties"", ""EstateAgentId"", ""EstateAgentName""
	FROM public.""Properties""	
	GROUP BY ""EstateAgentId"", ""EstateAgentName""
	ORDER BY ""TotalProperties"" DESC
	LIMIT 10;";

            var gardenSql = @"SELECT COUNT(""Id"") AS ""TotalProperties"", ""EstateAgentId"", ""EstateAgentName""
	FROM public.""Properties""
    WHERE ""HasGardenMention"" = true
	GROUP BY ""EstateAgentId"", ""EstateAgentName""
	ORDER BY ""TotalProperties"" DESC
	LIMIT 10;";

            using var connection = new Npgsql.NpgsqlConnection(dbContext.ConnectionString);

            await connection.OpenAsync(token);
            var rows = await connection.QueryAsync<ResultRow>(filterByHasGarden ? gardenSql : sql);

            return rows.ToList();
        }

        public async Task InsertPropertyInformationAsync(PropertyInformation property, CancellationToken token = default)
        {
            var sql = @"INSERT INTO public.""Properties""(""Id"", ""EstateAgentId"", ""EstateAgentName"", ""HasGardenMention"") 
VALUES (@Id, @EstateAgentId, @EstateAgentName, @HasGardenMention);";

            using var connection = new Npgsql.NpgsqlConnection(dbContext.ConnectionString);
            await connection.OpenAsync(token);
            await connection.ExecuteAsync(sql, property);
        }

        public async Task DeleteAllPropertiesAsync(CancellationToken token = default)
        {
            var sql = @"DELETE FROM public.""Properties"";";

            using var connection = new Npgsql.NpgsqlConnection(dbContext.ConnectionString);
            await connection.OpenAsync(token);
            await connection.ExecuteAsync(sql);
        }
    }
}
