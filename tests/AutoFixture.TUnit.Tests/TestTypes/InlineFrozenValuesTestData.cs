using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AutoFixture.TUnit.Tests.TestTypes;

[SuppressMessage("Usage", "TUnit0046:Return a `Func<T>` rather than a `<T>`")]
internal class InlineFrozenValuesTestData : InlineAttributeTestData<(BaseDataSourceAttribute attribute, MethodInfo testMethod, object[] expected)>
{
    public override IEnumerable<(BaseDataSourceAttribute attribute, MethodInfo testMethod, object[] expected)> GetData()
    {
        // All values provided by fixture
        yield return
        (
            CreateAttribute([], ("a", "string_a0"), ("b", "string_b0")),
            TestTypeWithMemberDataSource.GetTestWithFrozenParameter(),
            ["string_a0", "string_b0", "string_b0"]);

        // First parameter injected; Frozen parameter generated
        yield return
        (
            CreateAttribute(["string_a1"], ("b", "string_b1")),
            TestTypeWithMemberDataSource.GetTestWithFrozenParameter(),
            ["string_a1", "string_b1", "string_b1"]);

        // Frozen parameter is injected
        yield return
        (
            CreateAttribute(["string_a2", "string_b2"]),
            TestTypeWithMemberDataSource.GetTestWithFrozenParameter(),
            ["string_a2", "string_b2", "string_b2"]);
    }
}