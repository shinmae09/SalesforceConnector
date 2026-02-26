namespace SalesforceDataConnector.Domain.Configuration
{
    public sealed class SalesforceOptions
    {
        public string ClientId { get; init; } = default!;
        public string Username { get; init; } = default!;
        public string PrivateKey { get; init; } = default!;
        public string PrivateKeyPassphrase { get; init; } = default!;
        public string LoginUrl { get; init; } = default!;

        public string PrivateKeyPem { get; set; } = default!;
    }
}
