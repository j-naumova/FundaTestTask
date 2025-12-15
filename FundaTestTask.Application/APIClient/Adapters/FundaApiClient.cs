using FundaTestTask.Application.APIClient.Adapters.ExternalModels;
using FundaTestTask.Application.APIClient.Models;
using FundaTestTask.Application.APIClient.Ports;
using FundaTestTask.Application.Common.Models;
using FundaTestTask.Application.Output.Ports;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.RateLimit;
using Polly.Wrap;

namespace FundaTestTask.Application.APIClient.Adapters
{
    public class FundaApiClient : IPropertyApiClient
    {
        private const string UrlTemplate = "http://partnerapi.funda.nl/feeds/Aanbod.svc/json/{0}/?type=koop";
        private const string ParametersTemplate = "&zo=/amsterdam/&page={0}&pagesize={1}";
        private const string Paging = "Paging";
        private const string Objects = "Objects";
        private const int RetryCount = 3;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IApiOptions _apiOptions;
        private readonly IOutputService _outputService;

        private AsyncPolicyWrap _policyWrap;

        public FundaApiClient(IHttpClientFactory httpClientFactory, IApiOptions apiOptions, IOutputService outputService)
        {
            _httpClientFactory = httpClientFactory;
            _apiOptions = apiOptions;
            _outputService = outputService;

            //spec says (>100 per minute), so let's do one per second to be safe
            var rateLimitPolicy = Policy.RateLimitAsync(
                                    numberOfExecutions: 1,
                                    perTimeSpan: TimeSpan.FromSeconds(1));

            var retryPolicy = Policy
                            .Handle<RateLimitRejectedException>()
                            .WaitAndRetryAsync(
                                RetryCount,
                            (int _, Exception ex, Context __) => ((RateLimitRejectedException)ex).RetryAfter,
                            async (_, __, i, ____) => {
                                await outputService.Say($"Retrying: {i}");                            
                            });

            _policyWrap = Policy.WrapAsync(retryPolicy, rateLimitPolicy);
        }

        public async Task<ListPropertiesResponse> ListProperties(ListPropertiesRequest request, CancellationToken token = default)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var url = BuildUrl(request);
                var response = await _policyWrap.ExecuteAsync(async () => await httpClient.GetAsync(url, token));

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(token);
                    await _outputService.Say($"Error when getting data from the API, status: {response.StatusCode}, content: {errorContent}.");
                    throw new Exception(errorContent);
                }

                var content = await response.Content.ReadAsStringAsync(token);

                return ParseResponse(content);
            }
            catch (Exception ex) when (ex is not FormatException)
            {
                throw;
            }
        }

        private ListPropertiesResponse ParseResponse(string content)
        {
            var root = JObject.Parse(content);

            var pagingToken = root[Paging];
            if (pagingToken == null)
            {
                throw new FormatException("API returned unexpected object: missing Paging");
            }
            var paging = pagingToken.ToObject<FundaApiPaging>();

            if (paging == null)
            {
                throw new FormatException("API returned unexpected Paging format");
            }

            var objectsToken = root[Objects];
            if (objectsToken == null)
            {
                throw new FormatException("API returned unexpected object: missing Objects");
            }

            var properties = objectsToken.Select(o =>
            {
                var element = o.ToObject<FundaApiObject>();

                if (element == null)
                {
                    throw new FormatException("API returned unexpected object in Objects collection");
                }

                bool containsGarden = o.ToString().IndexOf("tuin", StringComparison.OrdinalIgnoreCase) >= 0;

                return new PropertyInformation
                {
                    EstateAgentName = element.MakelaarNaam,
                    EstateAgentId = element.MakelaarId,
                    Id = element.Id,
                    HasGardenMention = containsGarden
                };
            }).ToList();

            return new ListPropertiesResponse
            {
                Properties = properties,
                CurrentPage = paging.HuidigePagina,
                PagesNumber = paging.AantalPaginas
            };
        }

        private string BuildUrl(ListPropertiesRequest request)
        {
            var url = string.Format(UrlTemplate, _apiOptions.Key);
            var parameters = string.Format(ParametersTemplate, request.PageNumber, request.PageSize);
            return url + parameters;
        }
    }
}
