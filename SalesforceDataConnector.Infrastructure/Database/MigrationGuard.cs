using Microsoft.EntityFrameworkCore;

namespace SalesforceDataConnector.Infrastructure.Database
{
    public static class MigrationGuard
    {
        public static async Task<bool> HasPendingMigrationsAsync(AppDbContext db)
        {
            var pending = await db.Database.GetPendingMigrationsAsync();
            return pending.Any();
        }
    }
}
