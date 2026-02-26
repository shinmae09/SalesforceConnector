using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesforceDataConnector.Domain.Common.Extensions;
using SalesforceDataConnector.Infrastructure.Database;
using System.Net;

namespace SalesforceDataConnector.AzureFunctions.Functions.HttpTriggered;

public class MigrationFunction
{
    private readonly AppDbContext _db;
    private readonly ILogger<MigrationFunction> _logger;

    public MigrationFunction(AppDbContext db, ILogger<MigrationFunction> logger)
    {
        _db = db.ThrowIfNull(nameof(db));
        _logger = logger.ThrowIfNull(nameof(logger));
    }

    [Function("RunMigrations")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "salesforce/run-migrations")]
        HttpRequestData req)
    {
        try
        {
            _logger.LogInformation("Starting database migrations");

            if (Environment.GetEnvironmentVariable("ENABLE_DB_MIGRATIONS") != "true")
            {
                var forbidden = req.CreateResponse(HttpStatusCode.Forbidden);
                await forbidden.WriteStringAsync("Migrations disabled");
                return forbidden;
            }

            _logger.LogInformation("Migration request received");
            await MigrationLock.AcquireAsync(_db);

            if (!await MigrationGuard.HasPendingMigrationsAsync(_db))
            {
                var ok = req.CreateResponse(HttpStatusCode.OK);
                await ok.WriteStringAsync("No pending migrations");
                return ok;
            }

            _logger.LogInformation("Applying EF Core migrations");
            await _db.Database.MigrateAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Migrations applied successfully");

            return response;
        }
        catch (Exception ex) 
        {
            _logger.LogError("An error occurred when applying migrations in Azure SQL.");

            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync($"Failed to apply migrations. {ex.Message}");

            return response;
        }
    }
}