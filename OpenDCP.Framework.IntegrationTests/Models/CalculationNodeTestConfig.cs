using OpenDCP.Framework.Configuration.Model.Constants.CalculationNode;
using System.Text.Json.Nodes;

namespace OpenDCP.Framework.IntegrationTests.Models
{
    public class CalculationNodeTestConfig
    {
        public CalculationNodeType Type { get; set; }
        public string Name { get; set; }
        public JsonObject ConnectionString { get; set; }
    }
}
