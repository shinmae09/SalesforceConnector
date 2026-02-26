using SalesforceDataConnector.Domain.DTO;
using SalesforceDataConnector.Domain.Entities;
using SalesforceDataConnector.Domain.Models;

namespace SalesforceDataConnector.Domain.Abstractions.Salesforce
{
    public interface ISalesforceService
    {
        Task<IEnumerable<SalesforceServiceProvider>> GetServiceProvidersAsync();
        Task SyncRecordsToDatabaseAsync(IEnumerable<ServiceProvider> serviceProviders);
        Task<IEnumerable<OpportunityContactAlertDto>> GetOpportunityContactsWithAlertAsync(string serviceProviderId);
        Task<ServiceProviderDto> GetServiceProviderAsync(string name);
    }
}
