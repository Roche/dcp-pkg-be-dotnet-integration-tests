using OpenDCP.Framework.Configuration.Model.Constants.CalculationNode;
using System;

namespace OpenDCP.Framework.IntegrationTests.Models
{
    public class CalculationNodeTestMetaData
    {
        public CalculationNodeType Type { get; set; }
        public string Name { get; set; }
        public DateTime TestStartTime { get; set; }
    }
}
