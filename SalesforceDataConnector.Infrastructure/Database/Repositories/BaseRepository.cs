using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SalesforceDataConnector.Domain.Abstractions;
using SalesforceDataConnector.Domain.Common.Extensions;

namespace SalesforceDataConnector.Infrastructure.Database.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly AppDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext.ThrowIfNull(nameof(dbContext));
            _dbSet = _dbContext.Set<T>();
        }

        public async Task AddOrUpdateListAsync(IEnumerable<T> entities)
        {
            await _dbContext.BulkInsertOrUpdateAsync(entities);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
