using FundaTestTask.Application.APIClient.Models;

namespace FundaTestTask.Application.APIClient.Ports
{
    public interface IPropertyApiClient
    {
        Task<ListPropertiesResponse> ListProperties(ListPropertiesRequest request, CancellationToken token = default);
    }
}
