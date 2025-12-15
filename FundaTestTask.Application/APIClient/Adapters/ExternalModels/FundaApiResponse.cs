namespace FundaTestTask.Application.APIClient.Adapters.ExternalModels
{
    public class FundaApiPaging
    {
        public int AantalPaginas { get; set; }
        public int HuidigePagina { get; set; }
    }

    public class FundaApiObject
    {
        public int MakelaarId { get; set; }
        public required string MakelaarNaam { get; set; }
        public Guid Id { get; set; }
        public bool HasTuinMention { get; set; }
    }
}
