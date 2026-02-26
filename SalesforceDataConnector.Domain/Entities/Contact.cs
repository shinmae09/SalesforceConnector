using System.ComponentModel.DataAnnotations.Schema;

namespace SalesforceDataConnector.Domain.Entities
{
    [Table("Salesforce_Contact")]
    public class Contact
    {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public bool CanReceiveAlert { get; set; }

        public DateTime? SystemModstamp { get; set; }

        public DateTime? LastUpdate { get; set; }

        //Foreign Key
        public string? OpportunityId { get; set; }
    }
}
