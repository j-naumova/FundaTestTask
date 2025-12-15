using CsvHelper;
using FundaTestTask.Application.Files.Ports;
using System.Globalization;

namespace FundaTestTask.Application.Files.Adapters
{
    public class FileService : IFileService
    {
        public async Task SaveCsv<T>(string path, List<T> entries, CancellationToken cancellationToken = default)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                await csv.WriteRecordsAsync(entries, cancellationToken);
            }
        }
    }
}
