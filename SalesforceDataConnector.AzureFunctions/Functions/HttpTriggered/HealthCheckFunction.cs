using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesforceDataConnector.Domain.Abstractions.Salesforce;
using SalesforceDataConnector.Domain.Common.Extensions;
using SalesforceDataConnector.Infrastructure.Database;
using System.Net;

namespace SalesforceDataConnector.AzureFunctions.Functions.HttpTriggered;

public class HealthCheckFunction
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HealthCheckFunction> _logger;
    private readonly ISalesforceAuthProvider _authProvider;

    public HealthCheckFunction(
        AppDbContext dbContext,
        IConfiguration configuration,
        ILogger<HealthCheckFunction> logger,
        ISalesforceAuthProvider authProvider)
    {
        _dbContext = dbContext.ThrowIfNull(nameof(dbContext));
        _configuration = configuration.ThrowIfNull(nameof(configuration));
        _logger = logger.ThrowIfNull(nameof(logger));
        _authProvider = authProvider.ThrowIfNull(nameof(authProvider));
    }

    [Function("HealthCheckFunction")]
    public async Task<string> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "salesforce/services/health")] HttpRequestData req)
    {
        _logger.LogInformation("Checking SQL Database connection");

        var result = new Dictionary<string, string>();
        var response = req.CreateResponse();

        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync();
            result["SqlDatabase"] = canConnect ? "healthy" : "Unhealthy";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SQL Database health check failed.");
            result["SqlDatabase"] = $"Unhealthy: {ex.Message}";
        }

        try
        {
            var kvUri = _configuration["KEY_VAULT_URL"];
            if (string.IsNullOrWhiteSpace(kvUri))
            {
                result["KeyVault"] = "Skipped. KEY_VAULT_URL is not configured";
            }
            else
            {
                var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
                string secretName = "sql-connection-string";
                KeyVaultSecret secret = await client.GetSecretAsync(secretName);
                result["KeyVault"] = secret != null ? "Healthy" : "Unhealthy";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Key Vault health check failed");
            result["KeyVault"] = $"Unhealthy: {ex.Message}";
        }

        _logger.LogInformation("Checking Salesforce connection...");

        try
        {
            string token = await _authProvider.GetAccessTokenAsync();

            _logger.LogInformation("Salesforce connection successful. Token length: {Length}", token.Length);

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteStringAsync("Salesforce connection SUCCESS: Access token retrieved.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Salesforce connection FAILED.");
            result["Salesforce"] = $"Unhealthy: {ex.Message}";
        }

        var responseMessage = string.Join("; ", result.Select(kv => $"{kv.Key}: {kv.Value}"));
        _logger.LogInformation(responseMessage);

        response.StatusCode = HttpStatusCode.OK;
        await response.WriteStringAsync(responseMessage);

        return responseMessage;
    }
}