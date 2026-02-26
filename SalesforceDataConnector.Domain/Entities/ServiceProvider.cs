using System.ComponentModel.DataAnnotations.Schema;

namespace SalesforceDataConnector.Domain.Entities
{
    [Table("Salesforce_Service_Provider")]
    public class ServiceProvider
    {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public DateTime? SystemModstamp { get; set; }

        public List<Opportunity>? Opportunities { get; set; }

        public DateTime? LastUpdate { get; set; }
    }
}
