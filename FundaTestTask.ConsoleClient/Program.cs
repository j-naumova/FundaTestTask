using FundaTestTask.Application;
using FundaTestTask.Application.APIClient;
using FundaTestTask.Application.Data.Ports;
using FundaTestTask.Application.Output.Ports;
using FundaTestTask.ConsoleClient.ConsoleSpecific;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddPropertiesClient();
                services.AddData();
                services.AddFiles();
                services.AddSingleton<IOutputService, ConsoleOutputService>();
                services.AddSingleton<IWorkerOptions, WorkerOptions>();
                services.AddSingleton<IConnectionOptions, ConnectionOptions>();
                services.AddSingleton<IApiOptions, ApiOptions>();
                services.AddHostedService<DataWorker>();

            }).Build().Start();
    }
}