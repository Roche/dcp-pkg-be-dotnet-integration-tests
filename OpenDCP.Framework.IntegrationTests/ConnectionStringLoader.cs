using OpenDCP.Framework.Configuration.Model.Constants.Datasource;
using OpenDCP.Framework.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OpenDCP.Framework.IntegrationTests
{
    public static class ConnectionStringLoader
    {
        public static IEnumerable<object[]> GetSensorTelemetryConnections() => GetConnectionsOrYieldNullIfEmpty(DataType.SensorTelemetry);
        public static IEnumerable<object[]> GetAlarmConnections() => GetConnectionsOrYieldNullIfEmpty(DataType.Alarms);
        public static IEnumerable<object[]> GetAnalyticalConnections() => GetConnectionsOrYieldNullIfEmpty(DataType.AnalyticalData);
        public static IEnumerable<object[]> GetEventframeContextConnections() => GetConnectionsOrYieldNullIfEmpty(DataType.EventframeContext);
        public static IEnumerable<object[]> GetBatchGenealogyConnections() => GetConnectionsOrYieldNullIfEmpty(DataType.BatchGenealogy);
        public static IEnumerable<object[]> GetAssetHierarchyConnections() => GetConnectionsOrYieldNullIfEmpty(DataType.AssetHierarchy);
        public static IEnumerable<object[]> GetEDMSConnections() => GetConnectionsOrYieldNullIfEmpty(DataType.EDMS);
        public static IEnumerable<object[]> GetRecipeInformationConnections() => GetConnectionsOrYieldNullIfEmpty(DataType.RecipeInformation);
        public static IEnumerable<object[]> GetSpectralConnections() => GetConnectionsOrYieldNullIfEmpty(DataType.SpectralData);
        public static IEnumerable<object[]> GetRawMaterialConnections() => GetConnectionsOrYieldNullIfEmpty(DataType.RawMaterials);
        public static IEnumerable<object[]> GetMasterDataConnections() => GetConnectionsOrYieldNullIfEmpty(DataType.MasterData);
        public static IEnumerable<object[]> GetProcessMonitoringConnections() => GetConnectionsOrYieldNullIfEmpty(DataType.ProcessMonitoringData);

        // No change here
        public static IEnumerable<object[]> GetDatasourceConnectionStrings(DataType dataType)
        {
            var json = File.ReadAllText("Datasource/connection-strings.json");

            var datasources = JsonSerializer.Deserialize<List<DatasourceTestConfig>>(json)
                              ?.Where(x => x.DataType == dataType)
                              .ToList();

            if (datasources == null)
            {
                throw new ArgumentException("No connection strings provided");
            }

            if (!File.Exists("datasource-connection-string.json"))
            {
                var fields2remove = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "password" };

                var sanitizedDatasources = datasources.Select(x => new
                {
                    DataType = x.DataType.ToString(),
                    x.Name,
                    ConnectionType = x.ConnectionType.ToString(),
                    ConnectionString = RemoveFields(x.ConnectionString, fields2remove)
                });

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(sanitizedDatasources, options);
                File.WriteAllText("datasource-connection-string.json", jsonString);
            }

            foreach (var datasource in datasources)
            {
                yield return new object[] { datasource };
            }
        }

        private static JsonObject RemoveFields(JsonObject jsonObject, HashSet<string> keysToRemove)
        {
            var keysToDelete = new List<string>();

            foreach (var property in jsonObject)
            {
                if (keysToRemove.Contains(property.Key))
                {
                    keysToDelete.Add(property.Key);
                }
                else if (property.Value is JsonObject nestedObject)
                {
                    RemoveFields(nestedObject, keysToRemove);
                }
                else if (property.Value is JsonArray jsonArray)
                {
                    foreach (var item in jsonArray)
                    {
                        if (item is JsonObject arrayObject)
                        {
                            RemoveFields(arrayObject, keysToRemove);
                        }
                    }
                }
            }

            foreach (var key in keysToDelete)
            {
                jsonObject.Remove(key);
            }

            return jsonObject;
        }
        private static IEnumerable<object[]> GetConnectionsOrYieldNullIfEmpty(DataType dataType)
        {
            var configs = GetDatasourceConnectionStrings(dataType);

            if (!configs.Any())
            {
                // Yield one test case with null, so the test runs once and can skip itself.
                yield return new object[] { null };
                yield break;
            }

            foreach (var config in configs)
            {
                yield return config;
            }
        }
    }
}
