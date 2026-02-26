using SalesforceDataConnector.Domain.Abstractions;
using SalesforceDataConnector.Domain.Entities;

namespace SalesforceDataConnector.Domain.Abstractions.Salesforce
{
    public interface IOpportunityRepository : IBaseRepository<Opportunity>
    {
        Task<DateTime> GetMaxSystemModstampAsync();
    }
}
