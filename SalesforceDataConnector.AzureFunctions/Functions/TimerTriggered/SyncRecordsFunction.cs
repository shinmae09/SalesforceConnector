using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SalesforceDataConnector.Domain.Abstractions.Salesforce;
using SalesforceDataConnector.Domain.Common.Extensions;
using SalesforceDataConnector.Domain.Common.Mappers;

namespace SalesforceDataConnector.AzureFunctions.Functions.TimerTriggered;

public class SyncRecordsFunction
{
    private readonly ILogger<SyncRecordsFunction> _logger;
    private readonly ISalesforceService _salesforceService;

    public SyncRecordsFunction(
        ILogger<SyncRecordsFunction> logger,
        ISalesforceService salesforceService)
    {
        _logger = logger.ThrowIfNull(nameof(logger));
        _salesforceService = salesforceService.ThrowIfNull(nameof(salesforceService));
    }

    [Function("SyncRecordsFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "salesforce/sync-data")] HttpRequest req)
    {
        _logger.LogInformation("Starting to sync salesforce data now...");

        try
        {
            var salesforceServiceProviders = await _salesforceService.GetServiceProvidersAsync();
            var serviceProviders = SalesforceServiceProviderToDatabaseServiceProviderMapper.MapTo(salesforceServiceProviders);
            await _salesforceService.SyncRecordsToDatabaseAsync(serviceProviders);

            return new OkResult();
        }
        catch (Exception ex) 
        {
            _logger.LogError("An error was encountered when syncing salesforce data.");
            throw new InvalidOperationException($"Failed to sync salesforce data. {ex.Message}");
        }
    }
}