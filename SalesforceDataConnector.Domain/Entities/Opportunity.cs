using System.ComponentModel.DataAnnotations.Schema;

namespace SalesforceDataConnector.Domain.Entities
{
    [Table("Salesforce_Opportunity")]
    public class Opportunity
    {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public DateTime? SystemModstamp { get; set; }

        public List<Contact>? Contacts { get; set; }

        public DateTime? LastUpdate { get; set; }

        //Foreign Key
        public string? ServiceProviderId { get; set; }
    }
}
