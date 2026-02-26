using SalesforceDataConnector.Domain.Abstractions;
using SalesforceDataConnector.Domain.Entities;

namespace SalesforceDataConnector.Domain.Abstractions.Salesforce
{
    public interface IContactRepository : IBaseRepository<Contact>
    {
        Task<DateTime> GetMaxSystemModstampAsync();
    }
}
