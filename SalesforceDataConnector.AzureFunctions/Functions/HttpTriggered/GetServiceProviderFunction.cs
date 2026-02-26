using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SalesforceDataConnector.AzureFunctions.Services;
using SalesforceDataConnector.Domain.Abstractions.Salesforce;
using SalesforceDataConnector.Domain.Common.Extensions;

namespace SalesforceDataConnector.AzureFunctions.Functions.HttpTriggered;

public class GetServiceProviderFunction
{
    private readonly ILogger<GetServiceProviderFunction> _logger;
    private readonly ISalesforceService _salesforceService;

    public GetServiceProviderFunction(
        ILogger<GetServiceProviderFunction> logger,
        ISalesforceService salesforceService)
    {
        _logger = logger.ThrowIfNull(nameof(logger));
        _salesforceService = salesforceService.ThrowIfNull(nameof(salesforceService));
    }

    [Function("GetServiceProviderFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get",
        Route = "salesforce/service-provider")]
        HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string? name = req.Query["name"];
        if (string.IsNullOrWhiteSpace(name))
        {
            return new BadRequestResult();
        }

        var serviceProvider = await _salesforceService.GetServiceProviderAsync(name);

        return new OkObjectResult(serviceProvider);
    }
}