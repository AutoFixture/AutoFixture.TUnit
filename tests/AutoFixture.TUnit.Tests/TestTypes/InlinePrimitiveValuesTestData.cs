using System.Reflection;

namespace AutoFixture.TUnit.Tests.TestTypes;

internal class InlinePrimitiveValuesTestData : InlineAttributeTestData<(AutoFixtureDataSourceAttribute attribute, MethodInfo testMethod,
    object[] expected)>
{
    public override IEnumerable<(AutoFixtureDataSourceAttribute attribute, MethodInfo testMethod,
        object[] expected)> GetData()
    {
        // All values provided by fixture
        yield return
        (
            CreateAttributeWithFakeFixture([], ("a", "parameter_a1"), ("b", 31), ("c", 54.38m)),
            TestTypeWithMemberDataSource.GetMultipleValueTestMethodInfo(),
            ["parameter_a1", 31, 54.38m]);

        // Some parameters injected, some provided by fixture
        yield return
        (
            CreateAttributeWithFakeFixture(["parameter_a2"], ("b", 22), ("c", 817.218m)),
            TestTypeWithMemberDataSource.GetMultipleValueTestMethodInfo(),
            ["parameter_a2", 22, 817.218m]);

        // All parameters injected
        yield return
        (
            CreateAttributeWithFakeFixture(["parameter_a3", 13, 332.009m]),
            TestTypeWithMemberDataSource.GetMultipleValueTestMethodInfo(),
            ["parameter_a3", 13, 332.009m]);
    }
}