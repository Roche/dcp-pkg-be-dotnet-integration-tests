using OpenDCP.Framework.ConfigurationProvider.Constants;
using OpenDCP.Framework.ConfigurationProvider.Factories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OpenDCP.Framework.IntegrationTests.Builder
{
    public abstract class GenericBaseBuilder
    {
        protected JsonObject _connectionString;

        protected void GetConnectionString(string path)
        {
            var configuration = BuildTestConfiration();

            var secretConfigurationFactory = new SecretConfigurationProviderFactory();
            var secretConfigurationProvider = secretConfigurationFactory.Load("HashiCorpVault", null, configuration);

            var connectionString = secretConfigurationProvider
                .GetSecretAsJSON(path, configuration["SecretConfigurationProvider:Settings:MountPoints:AutomatedTesting"]);

            JsonObject result = JsonNode.Parse(connectionString.ToJsonString())!.AsObject();

            _connectionString = result;
        }

        protected void SetConnectionStringProperty(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key must not be null/empty.", nameof(key));

            var parts = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                throw new ArgumentException("Key must contain at least one segment.", nameof(key));

            JsonObject current = _connectionString;

            // Walk down until parent of last segment
            for (int i = 0; i < parts.Length - 1; i++)
            {
                var segment = parts[i];

                if (current[segment] is JsonObject existingObj)
                {
                    current = existingObj;
                    continue;
                }

                // Create or overwrite to allow nesting
                var newObj = new JsonObject();
                current[segment] = newObj;
                current = newObj;
            }

            var lastKey = parts[^1];

            // Store as JsonNode
            current[lastKey] = value is JsonNode node
                ? node
                : JsonValue.Create(value);
        }

        protected void GetConnectionString(string env, int id)
        {
            var configuration = BuildTestConfiration();

            var secretConfigurationFactory = new SecretConfigurationProviderFactory();
            var secretConfigurationProvider = secretConfigurationFactory.Load("HashiCorpVault", null, configuration);

            var connectionString = secretConfigurationProvider
                .GetSecretAsJSON($"{env}/{id}", configuration["SecretConfigurationProvider:Settings:MountPoints:DataSource"]);

            JsonObject result = JsonNode.Parse(connectionString.ToJsonString())!.AsObject();

            _connectionString = result;
        }

        protected void PublishConnectionString()
        {
            var connectionString = DeepCloneConnectionString();

            foreach (var fieldForRemove in GetSecretFields())
            {
                var key = connectionString
                 .Select(kvp => kvp.Key)
                 .FirstOrDefault(k => string.Equals(k, fieldForRemove, StringComparison.OrdinalIgnoreCase));

                if (key != null)
                {
                    connectionString[key] = string.Empty;
                }
            }

            string jsonString = connectionString.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = true // Optional: makes the output readable
            });


            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "connection-string.json");

            if (!File.Exists(outputPath))
            {
                File.WriteAllText(outputPath, jsonString);
            }
        }
        protected JsonObject DeepCloneConnectionString()
        {
            // Make a fresh copy of the connection string
            return JsonNode.Parse(_connectionString.ToJsonString()).AsObject();
        }

        protected IConfiguration BuildTestConfiration()
        {
            var configurationKeys = new Dictionary<string, string>
                {
                { "SecretConfigurationProvider:Settings:MountPoints:AutomatedTesting", "automated-testing/"},
                { "SecretConfigurationProvider:Settings:MountPoints:DataSource", "ext-datasource/"},
                { "SecretConfigurationProvider:Settings:Auth:CertificateName", Environment.GetEnvironmentVariable("TEST_CERT_NAME")},
                { "SecretConfigurationProvider:Settings:Address", "https://vault.service.roche.com"},
                { "SecretConfigurationProvider:Settings:Auth:Method", HashiCorpVaultAuthMethods.Certificate},
                { "SecretConfigurationProvider:Settings:Namespace", "dcp"},
            };

            var datasourceConfiguration = DatasourceConfiguration();

            if (datasourceConfiguration.Count > 0)
            {
                configurationKeys = configurationKeys
                    .Concat(datasourceConfiguration)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
            }

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationKeys)
                .Build();

            return configuration;
        }

        public virtual Dictionary<string, string> DatasourceConfiguration()
        {
            return new Dictionary<string, string>();
        }

        public abstract List<string> GetSecretFields();
    }



}
