using FundaTestTask.Application;
using FundaTestTask.Application.APIClient.Models;
using FundaTestTask.Application.APIClient.Ports;
using FundaTestTask.Application.Common.Models;
using FundaTestTask.Application.Data.Ports;
using FundaTestTask.Application.Files.Ports;
using FundaTestTask.Application.Output.Ports;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace FundaTestTask.Tests
{
    public class FlowTests
    {

        private ListPropertiesResponse _dataFromApi = new ListPropertiesResponse
        {
            CurrentPage = 1,
            PagesNumber = 1,
            Properties =
            [
                new PropertyInformation{
                    EstateAgentId = 1,
                    EstateAgentName = "test",
                    Id = Guid.NewGuid(),
                    HasGardenMention = true
                },
                new PropertyInformation{
                    EstateAgentId = 1,
                    EstateAgentName = "test",
                    Id = Guid.NewGuid(),
                    HasGardenMention = true
                },
                new PropertyInformation{
                    EstateAgentId = 2,
                    EstateAgentName = "test2",
                    Id = Guid.NewGuid(),
                    HasGardenMention = false
                }
            ]
        };

        private List<ResultRow> report = [
                new ResultRow{EstateAgentId = 1, EstateAgentName = "test", TotalProperties = 2},
                new ResultRow{EstateAgentId = 2, EstateAgentName = "test2", TotalProperties = 1}
                ];

        private List<ResultRow> gardenReport = [
            new ResultRow{EstateAgentId = 1, EstateAgentName = "test", TotalProperties = 2}
            ];

        [Fact]
        public async Task Flow_With_Getting_Fresh_Data()
        {
            var hostApplicationLifetimeMock = Substitute.For<IHostApplicationLifetime>();
            var propertyApiMock = Substitute.For<IPropertyApiClient>();
            var propertyInformationRepositoryMock = Substitute.For<IPropertyInformationRepository>();
            var fileServiceMock = Substitute.For<IFileService>();
            var outputServiceMock = Substitute.For<IOutputService>();
            var workerOptionsMock = Substitute.For<IWorkerOptions>();                     

            workerOptionsMock.ResetDatabase.Returns(true);

            propertyApiMock.ListProperties(Arg.Is<ListPropertiesRequest>( l => l.PageNumber == 1), Arg.Any<CancellationToken>()).Returns(_dataFromApi);

            propertyInformationRepositoryMock.GetReport(false, Arg.Any<CancellationToken>()).Returns(report);
            propertyInformationRepositoryMock.GetReport(true, Arg.Any<CancellationToken>()).Returns(gardenReport);

            var sut = new DataWorker(hostApplicationLifetimeMock, propertyApiMock,
                propertyInformationRepositoryMock, fileServiceMock, outputServiceMock, workerOptionsMock);

            await sut.StartAsync(CancellationToken.None);

            await propertyApiMock.Received(1).ListProperties(Arg.Is<ListPropertiesRequest>(l => l.PageNumber == 1), Arg.Any<CancellationToken>());
            await propertyInformationRepositoryMock.Received(1).InsertPropertyInformationAsync(_dataFromApi.Properties[0], Arg.Any<CancellationToken>());
            await propertyInformationRepositoryMock.Received(1).InsertPropertyInformationAsync(_dataFromApi.Properties[1], Arg.Any<CancellationToken>());
            await propertyInformationRepositoryMock.Received(1).InsertPropertyInformationAsync(_dataFromApi.Properties[2], Arg.Any<CancellationToken>());
            await propertyInformationRepositoryMock.Received(1).GetReport(false, Arg.Any<CancellationToken>());
            await propertyInformationRepositoryMock.Received(1).GetReport(true, Arg.Any<CancellationToken>());
            await fileServiceMock.Received(1).SaveCsv(Arg.Is<string>(s => s.Contains(".csv")), report, Arg.Any<CancellationToken>());
            await fileServiceMock.Received(1).SaveCsv(Arg.Is<string>(s => s.Contains(".csv") && s.Contains("Garden")), gardenReport, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Flow_With_Data_From_Db()
        {
            var hostApplicationLifetimeMock = Substitute.For<IHostApplicationLifetime>();
            var propertyApiMock = Substitute.For<IPropertyApiClient>();
            var propertyInformationRepositoryMock = Substitute.For<IPropertyInformationRepository>();
            var fileServiceMock = Substitute.For<IFileService>();
            var outputServiceMock = Substitute.For<IOutputService>();
            var workerOptionsMock = Substitute.For<IWorkerOptions>();

            workerOptionsMock.ResetDatabase.Returns(false);

            propertyInformationRepositoryMock.GetReport(false, Arg.Any<CancellationToken>()).Returns(report);
            propertyInformationRepositoryMock.GetReport(true, Arg.Any<CancellationToken>()).Returns(gardenReport);

            var sut = new DataWorker(hostApplicationLifetimeMock, propertyApiMock,
                propertyInformationRepositoryMock, fileServiceMock, outputServiceMock, workerOptionsMock);

            await sut.StartAsync(CancellationToken.None);

            await propertyInformationRepositoryMock.DidNotReceiveWithAnyArgs().DeleteAllPropertiesAsync(Arg.Any<CancellationToken>()); 
            await propertyApiMock.DidNotReceiveWithAnyArgs().ListProperties(Arg.Any<ListPropertiesRequest>(), Arg.Any<CancellationToken>());
            await propertyInformationRepositoryMock.DidNotReceiveWithAnyArgs().InsertPropertyInformationAsync(Arg.Any<PropertyInformation>(), Arg.Any<CancellationToken>());
            await propertyInformationRepositoryMock.Received(1).GetReport(false, Arg.Any<CancellationToken>());
            await propertyInformationRepositoryMock.Received(1).GetReport(true, Arg.Any<CancellationToken>());
            await fileServiceMock.Received(1).SaveCsv(Arg.Is<string>(s => s.Contains(".csv")), report, Arg.Any<CancellationToken>());
            await fileServiceMock.Received(1).SaveCsv(Arg.Is<string>(s => s.Contains(".csv") && s.Contains("Garden")), gardenReport, Arg.Any<CancellationToken>());
        }
    }
}