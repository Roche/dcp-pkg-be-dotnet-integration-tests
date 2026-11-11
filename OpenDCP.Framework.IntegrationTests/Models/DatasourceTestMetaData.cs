using OpenDCP.Framework.Configuration.Model.Constants.Datasource;
using System;

namespace OpenDCP.Framework.IntegrationTests.Models
{
    public class DatasourceTestMetaData
    {
        public DataType DataType { get; set; }
        public string Name { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public DateTime TestStartTime { get; set; }
    }
}
