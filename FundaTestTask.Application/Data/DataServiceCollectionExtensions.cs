using FundaTestTask.Application.Data.Adapters;
using FundaTestTask.Application.Data.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace FundaTestTask.Application.APIClient
{
    public static class DataServiceCollectionExtensions
    {
        public static IServiceCollection AddData(this IServiceCollection services)
        {
            services.AddSingleton<IDbContext, DapperDbContext>();
            services.AddSingleton<IPropertyInformationRepository, PropertyInformationRepository>();

            return services;
        }
    }
}
