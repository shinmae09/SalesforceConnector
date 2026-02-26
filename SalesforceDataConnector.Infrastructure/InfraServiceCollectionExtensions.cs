using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesforceDataConnector.Domain.Abstractions;
using SalesforceDataConnector.Domain.Abstractions.Salesforce;
using SalesforceDataConnector.Domain.Configuration;
using SalesforceDataConnector.Infrastructure.Clients;
using SalesforceDataConnector.Infrastructure.Database;
using SalesforceDataConnector.Infrastructure.Database.Repositories;
using System.Reflection;
using System.Security.Cryptography;

namespace SalesforceDataConnector.Infrastructure
{
    public static class InfraServiceCollectionExtensions
    {
        public static IServiceCollection RegisterInfrastructure(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            var salesforceOptions = BuildSalesforceOptions(configuration);
            services.AddSingleton(salesforceOptions);
            
            var connectionString = configuration["SQL_CONNECTION_STRING"];
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString, sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 6,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));
            }

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IOpportunityRepository, OpportunityRepository>();
            services.AddScoped<IServiceProviderRepository, ServiceProviderRepository>();

            services.AddHttpClient<ISalesforceAuthProvider, SalesforceAuthProvider>();
            services.AddHttpClient<ISalesforceClient, SalesforceClient>();

            return services;
        }

        // Helper method to build SalesforceOptions
        public static SalesforceOptions BuildSalesforceOptions(IConfiguration configuration)
        {
            var options = new SalesforceOptions
            {
                ClientId = configuration["SALESFORCE_CLIENT_ID"]
                    ?? throw new InvalidOperationException("SALESFORCE_CLIENT_ID is missing."),
                Username = configuration["SALESFORCE_USERNAME"]
                    ?? throw new InvalidOperationException("SALESFORCE_USERNAME is missing."),
                LoginUrl = configuration["SALESFORCE_API_BASE_URL"]!,
                PrivateKey = configuration["SALESFORCE_PRIVATE_KEY"]
                    ?? throw new InvalidOperationException("SALESFORCE_PRIVATE_KEY is missing."),
                PrivateKeyPassphrase = configuration["SALESFORCE_PRIVATE_KEY_PW"] ?? string.Empty
            };

            // Load PEM content from file, embedded resource, or inline
            options.PrivateKeyPem = LoadPem(options.PrivateKey);

            // Validate the PEM immediately to catch errors early
            ValidatePem(options.PrivateKeyPem, options.PrivateKeyPassphrase);

            return options;
        }

        private static void ValidatePem(string pemContent, string passphrase)
        {
            try
            {
                using var rsa = RSA.Create();
                if (!string.IsNullOrWhiteSpace(passphrase))
                    rsa.ImportFromEncryptedPem(pemContent.ToCharArray(), passphrase);
                else
                    rsa.ImportFromPem(pemContent.ToCharArray());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Salesforce PrivateKey PEM could not be loaded. Check the PEM content and passphrase.", ex);
            }
        }

        private static string LoadPem(string privateKeyPathOrContent)
        {
            string pem = string.Empty;

            // 1️⃣ Try as a file path relative to function root
            string resolvedPath = privateKeyPathOrContent;
            if (!File.Exists(resolvedPath))
            {
                resolvedPath = Path.Combine(Directory.GetCurrentDirectory(), privateKeyPathOrContent);
            }

            if (File.Exists(resolvedPath))
            {
                pem = File.ReadAllText(resolvedPath);
            }
            else
            {
                // 2️⃣ Try as embedded resource in any loaded assembly
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var asm in assemblies)
                {
                    var resourceName = asm.GetManifestResourceNames()
                        .FirstOrDefault(r => r.EndsWith(privateKeyPathOrContent, StringComparison.OrdinalIgnoreCase));
                    if (resourceName != null)
                    {
                        using var stream = asm.GetManifestResourceStream(resourceName)!;
                        using var reader = new StreamReader(stream);
                        pem = reader.ReadToEnd();
                        break;
                    }
                }

                // 3️⃣ Treat as inline PEM if still empty
                if (string.IsNullOrWhiteSpace(pem))
                {
                    pem = privateKeyPathOrContent;
                }
            }

            // Normalize newlines and trim
            pem = pem.Replace("\\n", "\n").Trim();

            // Validate quick sanity check
            if (!pem.Contains("PRIVATE KEY"))
            {
                throw new InvalidOperationException(
                    $"Salesforce PrivateKey PEM could not be loaded. Ensure it is:\n" +
                    $"- A valid file path relative to the function root\n" +
                    $"- An embedded resource (ending with {privateKeyPathOrContent})\n" +
                    $"- Or literal PEM content (must start with '-----BEGIN PRIVATE KEY-----')");
            }

            return pem;
        }
    }
}
