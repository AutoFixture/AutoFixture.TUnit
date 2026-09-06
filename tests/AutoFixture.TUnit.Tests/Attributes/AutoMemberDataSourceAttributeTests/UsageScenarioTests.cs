using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests.Attributes.AutoMemberDataSourceAttributeTests;

/// <summary>
/// End-to-end [AutoMemberDataSource] usage moved from the old Scenarios bucket.
/// </summary>
public class UsageScenarioTests
{
    [Test, AutoMemberDataSource(nameof(StringData))]
    public async Task WhenMemberSuppliesValues_UsesSuppliedValues(string s1, string s2)
    {
        await Assert.That(s1).IsEqualTo("foo");
        await Assert.That(s2).IsNotNull();
    }

    [Test, AutoMemberDataSource(nameof(StringData))]
    public async Task WhenMemberValuesPartial_SuppliesRemainingSpecimens(string s1, string s2, MyClass myClass)
    {
        await Assert.That(s1).IsEqualTo("foo");
        await Assert.That(s2).IsNotNull();
        await Assert.That(myClass).IsNotNull();
    }

    [Test, AutoMemberDataSource(nameof(StringData))]
    public async Task WhenMemberValuesPartial_DoesNotOverwriteSuppliedValues(string s1, string s2, string s3)
    {
        await Assert.That(s1).IsEqualTo("foo");
        await Assert.That(s2).IsNotNull();
        await Assert.That(s3).IsNotEqualTo("foo");
        await Assert.That(s3).IsNotEqualTo("bar");
    }

    [Test, AutoMemberDataSource(nameof(GetParametrizedData), 21, 38, 43)]
    public async Task WhenMemberDataParameterized_ReceivesExpectedData(int x, int y, int z)
    {
        await Assert.That(x).IsEqualTo(21);
        await Assert.That(y).IsEqualTo(38);
        await Assert.That(z).IsEqualTo(43);
    }

    [Test, MyCustomAutoMemberDataSource(nameof(IntData))]
    public async Task WhenCustomMemberAttribute_SuppliesExtraValues(int x, int y, int z)
    {
        await Assert.That(x).IsEqualTo(1337);
        await Assert.That(y).IsNotEqualTo(0);
        await Assert.That(z).IsEqualTo(42);
    }

    [Test, MyCustomAutoMemberDataSource(nameof(GetParametrizedData), 21, 38, 43)]
    public async Task WhenCustomMemberAttributeParameterized_ReceivesExpectedData(int x, int y, int z)
    {
        await Assert.That(x).IsEqualTo(21);
        await Assert.That(y).IsEqualTo(38);
        await Assert.That(z).IsEqualTo(43);
    }

    public static IEnumerable<object[]> StringData
    {
        get
        {
            yield return ["foo"];
            yield return ["foo", "bar"];
        }
    }

    public static IEnumerable<object[]> IntData
    {
        get
        {
            yield return [1337];
            yield return [1337, 7];
            yield return [1337, 7, 42];
        }
    }

    public static IEnumerable<object[]> GetParametrizedData(int x, int y, int z)
    {
        yield return [x, y, z];
    }

    public class MyCustomAutoMemberDataSourceAttribute : AutoMemberDataSourceAttribute
    {
        public MyCustomAutoMemberDataSourceAttribute(string memberName, params object[] parameters)
            : base(() => new Fixture().Customize(new TheAnswer()), memberName, parameters)
        {
        }
    }

    private class TheAnswer : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Inject(42);
        }
    }
}
