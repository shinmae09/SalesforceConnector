using Microsoft.Extensions.DependencyInjection;
using SalesforceDataConnector.AzureFunctions.Services;
using SalesforceDataConnector.Domain.Abstractions.Salesforce;

namespace SalesforceDataConnector.AzureFunctions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ISalesforceService, SalesforceService>();

            return services;
        }
    }
}
