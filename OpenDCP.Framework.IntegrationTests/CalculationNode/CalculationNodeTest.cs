using Microsoft.Extensions.Configuration;
using OpenDCP.Framework.IntegrationTests.Fixture;
using OpenDCP.Framework.IntegrationTests.Models;
using System;
using System.IO;
using System.Xml.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace OpenDCP.Framework.IntegrationTests.CalculationNode
{
    public class CalculationNodeTest : IClassFixture<ConfigurationFixture>
    {
        protected readonly IConfiguration _configuration;
        protected readonly ITestOutputHelper _output;

        public CalculationNodeTest(ConfigurationFixture fixture, ITestOutputHelper output)
        {
            _configuration = fixture.Configuration;
            _output = output;
        }

        protected void LogTestData(CalculationNodeTestConfig config)
        {
            var metaData = new CalculationNodeTestMetaData
            {
                TestStartTime = DateTime.UtcNow,
                Type = config.Type,
                Name = config.Name,
            };

            var serializer = new XmlSerializer(typeof(DatasourceTestMetaData));
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, metaData);
                _output.WriteLine(stringWriter.ToString());
            }
        }
    }
}
