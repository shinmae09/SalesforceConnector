using Microsoft.EntityFrameworkCore;
using SalesforceDataConnector.Domain.Abstractions.Salesforce;
using SalesforceDataConnector.Domain.DTO;
using SalesforceDataConnector.Domain.Entities;

namespace SalesforceDataConnector.Infrastructure.Database.Repositories
{
    public class ServiceProviderRepository : BaseRepository<ServiceProvider>, IServiceProviderRepository
    {
        public ServiceProviderRepository(AppDbContext dbContext) : base(dbContext)
        { 
        }

        public async Task<DateTime> GetMaxSystemModstampAsync()
        {
            return await _dbContext.ServiceProvider.MaxAsync(x => x.SystemModstamp) ?? DateTime.MinValue;
        }

        public async Task<IEnumerable<OpportunityContactAlertDto>> GetOpportunityContactsWithAlertAsync(string serviceProviderId)
        {
            var contacts = await _dbContext.ServiceProvider
                .Where(sp => sp.Id == serviceProviderId)
                .SelectMany(sp => sp.Opportunities!)
                .SelectMany(o => o.Contacts!)
                .Where(c => c.CanReceiveAlert)
                .Select(c => new OpportunityContactAlertDto(c.Name, c.Email))
                .AsNoTracking()
                .ToListAsync();

            return contacts;
        }

        public async Task<ServiceProviderDto> GetServiceProviderAsync(string name)
        {
            var serviceProvider = await _dbContext.ServiceProvider
                .Where(sp => sp.Name == name)
                .Select(sp => new ServiceProviderDto(sp.Id, sp.Name))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return serviceProvider;
        }
    }
}
