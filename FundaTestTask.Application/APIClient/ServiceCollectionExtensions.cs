using FundaTestTask.Application.APIClient.Adapters;
using FundaTestTask.Application.APIClient.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace FundaTestTask.Application.APIClient
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPropertiesClient(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IPropertyApiClient, FundaApiClient>();

            return services;
        }
    }
}
