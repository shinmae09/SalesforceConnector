using Newtonsoft.Json;
using SalesforceDataConnector.Domain.Abstractions.Salesforce;
using SalesforceDataConnector.Domain.Common.Extensions;
using SalesforceDataConnector.Domain.DTO;
using SalesforceDataConnector.Domain.Entities;
using SalesforceDataConnector.Domain.Models;

namespace SalesforceDataConnector.AzureFunctions.Services
{
    public class SalesforceService : ISalesforceService
    {
        private readonly ISalesforceClient _salesforceClient;
        private readonly IServiceProviderRepository _serviceProviderRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IContactRepository _contactRepository;

        public SalesforceService(
            ISalesforceClient salesforceClient,
            IServiceProviderRepository serviceProviderRepository,
            IOpportunityRepository opportunityRepository,
            IContactRepository contactRepository)
        {
            _salesforceClient = salesforceClient.ThrowIfNull(nameof(salesforceClient));
            _serviceProviderRepository = serviceProviderRepository.ThrowIfNull(nameof(serviceProviderRepository));
            _opportunityRepository = opportunityRepository.ThrowIfNull(nameof(opportunityRepository));
            _contactRepository = contactRepository.ThrowIfNull(nameof(contactRepository));
        }

        public async Task<IEnumerable<SalesforceServiceProvider>> GetServiceProvidersAsync()
        {
            List<SalesforceServiceProvider> salesforceServiceProviders = new List<SalesforceServiceProvider>();

            var query = @"
                SELECT Id, Name, SystemModstamp,
                    (SELECT Id, Name, SystemModstamp,
                        (SELECT Contact.Id, Contact.Name, Contact.Email, Contact.SystemModstamp, Contact.Alert_Receiver__c
                         FROM OpportunityContactRoles)
                     FROM Opportunities__r)
                FROM Service_Provider__c";

            string? nextRecordsUrl = null;
            do
            {
                var response = nextRecordsUrl is null ?
                    await _salesforceClient.QueryAsync(query) :
                    await _salesforceClient.QueryNextAsync(nextRecordsUrl);

                var deserializedResponse = JsonConvert.DeserializeObject<SalesforceServiceProviderWrapper>(response);
                if (deserializedResponse != null &&
                    deserializedResponse.records != null &&
                    deserializedResponse.records.Count != 0)
                {
                    salesforceServiceProviders.AddRange(deserializedResponse.records);

                    nextRecordsUrl = deserializedResponse!.done ? null : deserializedResponse.nextRecordsUrl;
                }
            }
            while (nextRecordsUrl is not null);

            return salesforceServiceProviders;
        }

        public async Task SyncRecordsToDatabaseAsync(IEnumerable<ServiceProvider> serviceProviders)
        {
            var opportunities = serviceProviders
            .SelectMany(sp => sp.Opportunities ?? Enumerable.Empty<Opportunity>())
            .DistinctBy(opp => opp.Id);
            var contacts = opportunities
                .SelectMany(opp => opp.Contacts ?? Enumerable.Empty<Contact>())
                .DistinctBy(con => con.Id);

            var spMaxModstamp = await _serviceProviderRepository.GetMaxSystemModstampAsync();
            var oppMaxModstamp = await _opportunityRepository.GetMaxSystemModstampAsync();
            var conMaxModstamp = await _contactRepository.GetMaxSystemModstampAsync();

            var currentDateTime = DateTime.UtcNow;

            var serviceProvidersFiltered = serviceProviders
                .Where(sp => sp.SystemModstamp > spMaxModstamp)
                .Select(sp =>
                {
                    sp.LastUpdate = currentDateTime;
                    return sp;
                });
            if (serviceProvidersFiltered.Any())
            {
                await _serviceProviderRepository.AddOrUpdateListAsync(serviceProvidersFiltered);
            }

            var opportunitiesFiltered = opportunities
                .Where(opp => opp.SystemModstamp > oppMaxModstamp)
                .Select(opp =>
                {
                    opp.LastUpdate = currentDateTime;
                    return opp;
                });
            if (opportunitiesFiltered.Any())
            {
                await _opportunityRepository.AddOrUpdateListAsync(opportunitiesFiltered);
            }

            var contactsFiltered = contacts
                .Where(con => con.SystemModstamp > conMaxModstamp)
                .Select(con =>
                {
                    con.LastUpdate = currentDateTime;
                    return con;
                });
            if (contactsFiltered.Any())
            {
                await _contactRepository.AddOrUpdateListAsync(contactsFiltered);
            }
        }

        public async Task<IEnumerable<OpportunityContactAlertDto>> GetOpportunityContactsWithAlertAsync(string serviceProviderId)
        {
            return await _serviceProviderRepository.GetOpportunityContactsWithAlertAsync(serviceProviderId);
        }

        public async Task<ServiceProviderDto> GetServiceProviderAsync(string name)
        {
            return await _serviceProviderRepository.GetServiceProviderAsync(name);
        }
    }
}
