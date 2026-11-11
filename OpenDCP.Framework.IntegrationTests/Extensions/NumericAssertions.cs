using Xunit;

namespace OpenDCP.Framework.IntegrationTests.Extensions
{
    public static class NumericAssertions
    {

        public static void BeNumeric(this object actual)
        {
            // Check if the actual object is a numeric type
            var isNumeric = actual is byte || actual is sbyte || actual is short || actual is ushort ||
                            actual is int || actual is uint || actual is long || actual is ulong ||
                            actual is float || actual is double || actual is decimal;

            // Assert that the value is numeric
            Assert.True(isNumeric, $"because {actual} should be numeric");
        }
    }
}
