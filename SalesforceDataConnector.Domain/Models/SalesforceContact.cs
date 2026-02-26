using Newtonsoft.Json;

namespace SalesforceDataConnector.Domain.Models
{
    public class SalesforceOpportunityContactWrapper
    {
        public List<SalesforceOpportunityContact>? records { get; set; }
    }

    public class SalesforceOpportunityContact
    {
        public SalesforceContact? Contact { get; set; }
    }

    public class SalesforceContact
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime? SystemModstamp { get; set; }

        [JsonProperty("Alert_Receiver__c")]
        public bool AlertReceiver { get; set; }
    }
}
