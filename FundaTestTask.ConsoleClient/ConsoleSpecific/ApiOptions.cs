using FundaTestTask.Application.APIClient;

namespace FundaTestTask.ConsoleClient.ConsoleSpecific
{
    internal class ApiOptions : IApiOptions
    {
        public string Key => ConfigHelper.GetProperty(nameof(Key));
    }
}
