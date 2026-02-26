using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SalesforceDataConnector.Domain.Abstractions.Salesforce;
using SalesforceDataConnector.Domain.Common.Extensions;

namespace SalesforceDataConnector.AzureFunctions.Functions.HttpTriggered;

public class GetOpportunityContactsFunction
{
    private readonly ILogger<GetOpportunityContactsFunction> _logger;
    private readonly ISalesforceService _salesforceService;

    public GetOpportunityContactsFunction(
        ILogger<GetOpportunityContactsFunction> logger,
        ISalesforceService salesforceService)
    {
        _logger = logger.ThrowIfNull(nameof(logger));
        _salesforceService = salesforceService.ThrowIfNull(nameof(salesforceService));
    }

    [Function("GetOpportunityContactsFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get",
        Route = "salesforce/service-provider/{serviceProviderId}/opportunity/contact-alert")]
        HttpRequest req,
        string serviceProviderId)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        if (string.IsNullOrWhiteSpace(serviceProviderId))
        {
            return new BadRequestResult();
        }

        var contacts = await _salesforceService.GetOpportunityContactsWithAlertAsync(serviceProviderId);

        return new OkObjectResult(contacts);
    }
}