using FundaTestTask.Application.APIClient.Models;
using FundaTestTask.Application.APIClient.Ports;
using FundaTestTask.Application.Data.Ports;
using FundaTestTask.Application.Files.Ports;
using FundaTestTask.Application.Output.Ports;
using Microsoft.Extensions.Hosting;

namespace FundaTestTask.Application
{
    public class DataWorker(IHostApplicationLifetime applicationLifetime,
        IPropertyApiClient propertyApiClient,
        IPropertyInformationRepository repository,
        IFileService fileService,
        IOutputService outputService,
        IWorkerOptions workerOptions) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (workerOptions.ResetDatabase)
            {
                await outputService.Say("Resetting DB and getting fresh data from API.");
                await GetFreshData(cancellationToken);
            }

            await outputService.Say("Generating report without garden filter.");
            var report = await repository.GetReport(filterByHasGarden: false, cancellationToken);

            if (report.Count == 0) 
            {
                await outputService.Say("Report is empty! Check if you need to change configuration to get fresh data.");
            }
            else
            {
                await outputService.Say("Generating report with garden filter.");
                var reportGarden = await repository.GetReport(filterByHasGarden: true, cancellationToken);

                await outputService.Say("Saving report without garden filter.");
                var path = Directory.GetCurrentDirectory() + "/top10.csv";
                await fileService.SaveCsv(path, report, cancellationToken);

                await outputService.Say("Saving report with garden filter.");
                var pathGarden = Directory.GetCurrentDirectory() + "/top10Garden.csv";
                await fileService.SaveCsv(pathGarden, reportGarden, cancellationToken);
            }                

            await outputService.Say("Stopping application...");

            applicationLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task GetFreshData(CancellationToken cancellationToken)
        {
            try
            {
                await repository.DeleteAllPropertiesAsync(cancellationToken);

                int nextPage = 1;

                do
                {
                    var request = new ListPropertiesRequest
                    {
                        PageNumber = nextPage,
                        PageSize = 25
                    };

                    await outputService.Say($"Getting page #{nextPage} of size {request.PageSize}");

                    var response = await propertyApiClient.ListProperties(request, cancellationToken);
                    nextPage = await ProcessPropertiesPage(response, cancellationToken);                    

                } while (nextPage > 0);
            }
            catch (Exception ex)
            {
                await outputService.Say($"Got exception while trying to get fresh data: {ex.Message}");
            }
        }

        private async Task<int> ProcessPropertiesPage(ListPropertiesResponse response, CancellationToken cancellationToken)
        {
            foreach (var property in response.Properties)
            {
                await outputService.Say($"Inserting entry with Id {property.Id}");
                await repository.InsertPropertyInformationAsync(property, cancellationToken);
            }

            if (response.CurrentPage < response.PagesNumber)
            {
                return response.CurrentPage + 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
