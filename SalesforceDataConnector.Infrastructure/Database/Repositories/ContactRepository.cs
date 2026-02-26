using Microsoft.EntityFrameworkCore;
using SalesforceDataConnector.Domain.Abstractions.Salesforce;
using SalesforceDataConnector.Domain.Entities;

namespace SalesforceDataConnector.Infrastructure.Database.Repositories
{
    public class ContactRepository : BaseRepository<Contact>, IContactRepository
    {
        public ContactRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<DateTime> GetMaxSystemModstampAsync()
        {
            return await _dbContext.Contacts.MaxAsync(x => x.SystemModstamp) ?? DateTime.MinValue;
        }
    }
}
