using Microsoft.IdentityModel.Tokens;
using SalesforceDataConnector.Domain.Abstractions.Salesforce;
using SalesforceDataConnector.Domain.Common.Extensions;
using SalesforceDataConnector.Domain.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

namespace SalesforceDataConnector.Infrastructure.Clients
{
    public class SalesforceAuthProvider : ISalesforceAuthProvider
    {
        private readonly SalesforceOptions _options;
        private readonly HttpClient _httpClient;

        public SalesforceAuthProvider(SalesforceOptions options, HttpClient httpClient)
        {
            _options = options.ThrowIfNull(nameof(options));
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var rsa = RSA.Create();
            if (!string.IsNullOrWhiteSpace(_options.PrivateKeyPassphrase))
            {
                rsa.ImportFromEncryptedPem(_options.PrivateKeyPem.ToCharArray(), _options.PrivateKeyPassphrase);
            }
            else
            {
                rsa.ImportFromPem(_options.PrivateKeyPem.ToCharArray());
            }

            var now = DateTime.UtcNow;
            var jwtToken = new JwtSecurityToken(
                issuer: _options.ClientId,
                audience: _options.LoginUrl,
                claims: new[] { new Claim("sub", _options.Username) },
                notBefore: now,
                expires: now.AddMinutes(3),
                signingCredentials: new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            );

            var handler = new JwtSecurityTokenHandler();
            var assertion = handler.WriteToken(jwtToken);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new KeyValuePair<string, string>("assertion", assertion)
            });

            var tokenEndpoint = $"{_options.LoginUrl}/services/oauth2/token";
            var response = await _httpClient.PostAsync(tokenEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Salesforce token request failed: {response.StatusCode} - {errorBody}");
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (!json.TryGetProperty("access_token", out var accessTokenProp))
            {
                throw new InvalidOperationException("Salesforce response did not contain access_token.");
            }

            return accessTokenProp.GetString()!;
        }
    }
}
