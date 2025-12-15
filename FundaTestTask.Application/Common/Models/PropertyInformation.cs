namespace FundaTestTask.Application.Common.Models
{
    public class PropertyInformation
    {
        public Guid Id { get; set; }
        public int EstateAgentId { get; set; }
        public required string EstateAgentName { get; set; }
        public bool HasGardenMention { get; set; }
    }
}
