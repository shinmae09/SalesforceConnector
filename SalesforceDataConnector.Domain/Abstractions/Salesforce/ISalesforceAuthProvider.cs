namespace SalesforceDataConnector.Domain.Abstractions.Salesforce
{
    public interface ISalesforceAuthProvider
    {
        Task<string> GetAccessTokenAsync();
    }
}
