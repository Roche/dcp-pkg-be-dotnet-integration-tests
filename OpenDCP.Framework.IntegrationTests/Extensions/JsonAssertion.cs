using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace OpenDCP.Framework.IntegrationTests.Extensions
{
    public static class JsonAssertions
    {
        public static JsonAssertion Should(this object obj) => new JsonAssertion(obj);
    }

    public class JsonAssertion
    {
        private readonly object _obj;

        public JsonAssertion(object obj)
        {
            _obj = obj;
        }

        public JsonAssertion BeNumericJsonValue()
        {
            if (_obj is JsonValue jsonValue)
            {
                var element = jsonValue.GetValue<JsonElement>();
                Assert.True(element.ValueKind == JsonValueKind.Number, "Expected JSON value to be a number.");
            }
            else
            {
                Assert.True(false, "Object is not a JsonValue.");
            }

            return this;
        }

        public JsonAssertion NotBeNumericJsonValue()
        {
            if (_obj is JsonValue jsonValue)
            {
                var element = jsonValue.GetValue<JsonElement>();
                Assert.True(element.ValueKind != JsonValueKind.Number, "Expected JSON value not to be a number.");
            }
            else
            {
                // If it's not even a JsonValue, then it's definitely not numeric
                return this;
            }

            return this;
        }
    }
}
