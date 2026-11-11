using Microsoft.Extensions.Configuration;
using System;
using System.IO;


namespace OpenDCP.Framework.IntegrationTests.Fixture
{
    public class ConfigurationFixture : IDisposable
    {
        public IConfiguration Configuration { get; private set; }

        public ConfigurationFixture()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Set base path for JSON files
                .AddJsonFile("appsettings.json",
                              optional: true,
                              reloadOnChange: true
                 )
                .Build();
        }

        public void Dispose()
        {
            // Cleanup if necessary
        }
    }
}
