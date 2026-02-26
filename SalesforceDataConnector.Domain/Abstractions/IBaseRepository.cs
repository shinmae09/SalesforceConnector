namespace SalesforceDataConnector.Domain.Abstractions
{
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        /// Get all Entities of Type T
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Inserts or updates the entities provided.
        /// </summary>
        /// <returns></returns>
        Task AddOrUpdateListAsync(IEnumerable<T> entities);
    }
}
