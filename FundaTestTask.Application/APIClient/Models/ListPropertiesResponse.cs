using FundaTestTask.Application.Common.Models;
namespace FundaTestTask.Application.APIClient.Models
{
    public class ListPropertiesResponse
    {
        public required List<PropertyInformation> Properties { get; set; }

        public int CurrentPage { get; set; }
        public int PagesNumber { get; set; }
    }
}
