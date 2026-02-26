using Newtonsoft.Json;

namespace SalesforceDataConnector.Domain.Models
{
    public class SalesforceOpportunityWrapper
    {
        public List<SalesforceOpportunity>? records { get; set; }
    }

    public class SalesforceOpportunity
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public DateTime? SystemModstamp { get; set; }

        [JsonProperty("OpportunityContactRoles")]
        public SalesforceOpportunityContactWrapper? Contacts { get; set; }
    }
}
