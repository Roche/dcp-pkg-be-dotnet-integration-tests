using OpenDCP.Framework.Configuration.Model.Constants.Datasource;
using System.Text.Json.Nodes;

namespace OpenDCP.Framework.IntegrationTests.Models
{
    public class DatasourceTestConfig
    {
        public DataType DataType { get; set; }
        public string Name { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public JsonObject ConnectionString { get; set; }
    }
}
