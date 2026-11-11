using System;
using System.Linq;
using Xunit;

namespace OpenDCP.Framework.IntegrationTests.Tests
{
    public class TestContractCoverage<TBase>
    {
        [Fact]
        public void AllTestContractsHaveImplementations()
        {
            // Adjust to the assembly where your contracts live:
            var baseContractType = typeof(TBase);

            var contractAssembly = baseContractType.Assembly;

            // The assembly containing the *concrete derived class* (not this generic base)
            var currentAssembly = GetType().Assembly;

            // 1. Find all generic test contract base types
            var contractTypes = contractAssembly
                .GetTypes()
                .Where(t =>
                    t.IsClass &&
                    IsSubclassOfRawGeneric(t, baseContractType) &&
                    t.GetCustomAttributes(typeof(CollectionAttribute), true).Any() == false &&
                    t.Name.Contains("TestContract") && !t.Name.Contains("BaseTestContract")
                )
                .ToList();

            // 3. Check which contracts are missing
            var implementedContracts = currentAssembly
                .GetTypes()
                .Where(t => t.BaseType != null && !t.IsAbstract)
                .Select(t =>
                {
                    var baseType = t.BaseType!;
                    return baseType.IsGenericType
                        ? baseType.GetGenericTypeDefinition()
                        : baseType;
                })
                .Distinct()
                .ToHashSet();

            var missingContracts = contractTypes
                .Where(c => !implementedContracts.Contains(
                    c.IsGenericType ? c.GetGenericTypeDefinition() : c))
                .ToList();


            // 4. Assert
            Assert.True(!missingContracts.Any(),
                "Missing implementations for test contracts:\n" +
                string.Join("\n", missingContracts.Select(c => c.FullName)));
        }

        /// <summary>
        /// Checks if a type derives from a given generic base (open or closed).
        /// </summary>
        private static bool IsSubclassOfRawGeneric(Type toCheck, Type baseType)
        {
            var genericBase = baseType.IsGenericType
                ? baseType.GetGenericTypeDefinition()
                : baseType;

            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType
                    ? toCheck.GetGenericTypeDefinition()
                    : toCheck;
                if (cur == genericBase)
                    return true;
                toCheck = toCheck.BaseType!;
            }
            return false;
        }
    }

}
