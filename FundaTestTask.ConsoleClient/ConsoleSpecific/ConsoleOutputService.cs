using FundaTestTask.Application.Output.Ports;

namespace FundaTestTask.ConsoleClient.ConsoleSpecific
{
    internal class ConsoleOutputService : IOutputService
    {
        public Task Say(string message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
