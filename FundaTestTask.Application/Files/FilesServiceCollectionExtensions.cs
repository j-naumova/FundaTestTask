using FundaTestTask.Application.Files.Adapters;
using FundaTestTask.Application.Files.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace FundaTestTask.Application.APIClient
{
    public static class FilesServiceCollectionExtensions
    {
        public static IServiceCollection AddFiles(this IServiceCollection services)
        {                           
            services.AddSingleton<IFileService, FileService>();

            return services;
        }
    }
}
