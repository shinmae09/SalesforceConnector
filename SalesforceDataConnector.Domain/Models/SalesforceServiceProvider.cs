using Newtonsoft.Json;

namespace SalesforceDataConnector.Domain.Models
{
    public class SalesforceServiceProviderWrapper
    {
        public List<SalesforceServiceProvider>? records { get; set; }
        public bool done { get; set; }
        public string? nextRecordsUrl { get; set; }
    }

    public class SalesforceServiceProvider
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public DateTime? SystemModstamp { get; set; }

        [JsonProperty("Opportunities__r")]
        public SalesforceOpportunityWrapper? Opportunities { get; set; }
    }
}
