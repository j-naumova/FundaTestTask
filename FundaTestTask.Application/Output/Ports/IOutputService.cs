namespace FundaTestTask.Application.Output.Ports
{
    public interface IOutputService
    {
        Task Say(string message, CancellationToken cancellationToken = default);
    }
}
