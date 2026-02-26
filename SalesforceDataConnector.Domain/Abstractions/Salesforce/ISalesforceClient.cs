using System.Text.Json;

namespace SalesforceDataConnector.Domain.Abstractions.Salesforce
{
    public interface ISalesforceClient
    {
        Task<string> QueryAsync(string query);
        Task<string> QueryNextAsync(string nextRecordsUrl);
    }
}
