using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Xml.Serialization;
using Xunit.Abstractions;
using Xunit;
using OpenDCP.Framework.IntegrationTests.Fixture;
using OpenDCP.Framework.IntegrationTests.Models;

namespace OpenDCP.Framework.IntegrationTests.Datasource
{
    public class DatasourceTest : IClassFixture<ConfigurationFixture>
    {
        protected readonly IConfiguration _configuration;
        protected readonly ITestOutputHelper _output;

        public DatasourceTest(ConfigurationFixture fixture, ITestOutputHelper output)
        {
            _configuration = fixture.Configuration;
            _output = output;
        }

        protected void LogTestData(DatasourceTestConfig config)
        {
            var metaData = new DatasourceTestMetaData
            {
                TestStartTime = DateTime.UtcNow,
                DataType = config.DataType,
                ConnectionType = config.ConnectionType,
                Name = config.Name,
            };

            var serializer = new XmlSerializer(typeof(DatasourceTestMetaData));
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, metaData);
                _output.WriteLine(stringWriter.ToString());
            }
        }
        protected void VerifyTestConnectionString(DatasourceTestConfig config)
        {
            Skip.If(config == null, "Skipping because no valid data was provided");
        }
    }
}
