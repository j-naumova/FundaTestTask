namespace FundaTestTask.Application.Files.Ports
{
    public interface IFileService
    {
        Task SaveCsv<T>(string path, List<T> entries, CancellationToken cancellationToken = default);
    }
}
