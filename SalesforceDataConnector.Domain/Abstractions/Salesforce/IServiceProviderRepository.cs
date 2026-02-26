using SalesforceDataConnector.Domain.Abstractions;
using SalesforceDataConnector.Domain.DTO;
using SalesforceDataConnector.Domain.Entities;

namespace SalesforceDataConnector.Domain.Abstractions.Salesforce
{
    public interface IServiceProviderRepository : IBaseRepository<ServiceProvider>
    {
        Task<DateTime> GetMaxSystemModstampAsync();
        Task<IEnumerable<OpportunityContactAlertDto>> GetOpportunityContactsWithAlertAsync(string serviceProviderId);
        Task<ServiceProviderDto> GetServiceProviderAsync(string name);
    }
}
