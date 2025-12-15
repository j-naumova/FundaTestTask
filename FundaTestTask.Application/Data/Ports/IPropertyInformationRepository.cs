using FundaTestTask.Application.Common.Models;

namespace FundaTestTask.Application.Data.Ports
{
    public interface IPropertyInformationRepository
    {
        Task InsertPropertyInformationAsync(PropertyInformation property, CancellationToken token = default);
        Task<List<ResultRow>> GetReport(bool filterByHasGarden, CancellationToken token = default);
        Task DeleteAllPropertiesAsync(CancellationToken token = default);
    }
}
