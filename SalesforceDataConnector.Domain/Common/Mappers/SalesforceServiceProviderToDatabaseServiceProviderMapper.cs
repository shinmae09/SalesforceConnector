using SalesforceDataConnector.Domain.Entities;
using SalesforceDataConnector.Domain.Models;

namespace SalesforceDataConnector.Domain.Common.Mappers
{
    public static class SalesforceServiceProviderToDatabaseServiceProviderMapper
    {
        public static ServiceProvider MapTo(SalesforceServiceProvider ssp)
        {
            var serviceProvider = new ServiceProvider()
            {
                Id = ssp.Id!,
                Name = ssp.Name!,
                SystemModstamp = ssp.SystemModstamp,
            };

            if (ssp.Opportunities != null && ssp.Opportunities.records != null)
            {
                serviceProvider.Opportunities = new List<Opportunity>();

                foreach (var opp in ssp.Opportunities.records)
                {
                    var opportunity = new Opportunity()
                    {
                        Id = opp.Id!,
                        Name = opp.Name!,
                        SystemModstamp = opp.SystemModstamp,
                        ServiceProviderId = serviceProvider.Id,
                    };
                    serviceProvider.Opportunities.Add(opportunity);

                    if (opp.Contacts != null && opp.Contacts.records != null)
                    {
                        opportunity.Contacts = new List<Contact>();

                        foreach (var con in opp.Contacts.records)
                        {
                            if (string.IsNullOrWhiteSpace(con.Contact?.Email))
                            {
                                continue;
                            }

                            var contact = new Contact()
                            {
                                Id = con.Contact?.Id!,
                                Name = con.Contact?.Name!,
                                Email = con.Contact?.Email!,
                                SystemModstamp = con.Contact?.SystemModstamp,
                                CanReceiveAlert = con.Contact != null ? con.Contact.AlertReceiver : false,
                                OpportunityId = opportunity.Id,
                            };
                            opportunity.Contacts.Add(contact);
                        }
                    }
                }
            }

            return serviceProvider;
        }

        public static IEnumerable<ServiceProvider> MapTo(IEnumerable<SalesforceServiceProvider> salesforceServiceProviders)
        {
            if (salesforceServiceProviders == null)
            {
                return Enumerable.Empty<ServiceProvider>();
            }

            return salesforceServiceProviders.Select(MapTo);
        }
    }
}
