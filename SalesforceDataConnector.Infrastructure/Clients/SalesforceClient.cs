using SalesforceDataConnector.Domain.Abstractions.Salesforce;
using SalesforceDataConnector.Domain.Common.Extensions;
using SalesforceDataConnector.Domain.Configuration;
using System.Net.Http.Headers;

namespace SalesforceDataConnector.Infrastructure.Clients
{
    public class SalesforceClient : ISalesforceClient
    {
        private const string SALESFORCE_API_VERSION = "65.0";

        private readonly ISalesforceAuthProvider _authProvider;
        private readonly HttpClient _httpClient;
        private readonly SalesforceOptions _salesforce;

        public SalesforceClient(
            ISalesforceAuthProvider authProvider, 
            HttpClient httpClient,
            SalesforceOptions salesforce)
        {
            _authProvider = authProvider.ThrowIfNull(nameof(authProvider));
            _httpClient = httpClient ?? new HttpClient();
            _salesforce = salesforce.ThrowIfNull(nameof(_salesforce));
        }

        public async Task<string> QueryAsync(string query)
        {
            var accessToken = await _authProvider.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"{_salesforce.LoginUrl}/services/data/v{SALESFORCE_API_VERSION}/query/?q={Uri.EscapeDataString(query)}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> QueryNextAsync(string nextRecordsUrl)
        {
            var accessToken = await _authProvider.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"{_salesforce.LoginUrl}{nextRecordsUrl}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
