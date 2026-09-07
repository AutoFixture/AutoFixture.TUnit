using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests.Attributes.AutoMemberDataSourceAttributeTests;

/// <summary>
/// End-to-end [AutoMemberDataSource&lt;T&gt;] usage parallel to the non-generic scenarios.
/// </summary>
public class GenericUsageScenarioTests
{
    [Test, AutoMemberDataSource<MemberHost>(nameof(MemberHost.Strings))]
    public async Task WhenGenericMemberYieldsStrings_ReceivesValueAndFillsRemaining(string value, MyClass leftover)
    {
        await Assert.That<string[]>(["one", "two"]).Contains(value);
        await Assert.That(leftover).IsNotNull();
    }

    [Test, AutoMemberDataSource<MemberHost>(nameof(MemberHost.ObjectArrays))]
    public async Task WhenGenericMemberReturnsObjectArrays_UsesSuppliedValues(
        string a, int b, RecordType<string> c)
    {
        await Assert.That<(string, int, string)[]>([
            ("hello", 1, "world"),
            ("foo", 2, "bar")
        ]).Contains((a, b, c.Value));
    }

    [Test, AutoMemberDataSource<MemberHost>(nameof(MemberHost.StringData))]
    public async Task WhenGenericMemberOnHostType_UsesSuppliedValues(string s1, string s2, MyClass leftover)
    {
        await Assert.That(s1).IsEqualTo("foo");
        await Assert.That(s2).IsNotNull();
        await Assert.That(leftover).IsNotNull();
    }

    [Test, MyCustomAutoMemberDataSource<MemberHost>(nameof(MemberHost.IntData))]
    public async Task WhenGenericCustomMemberAttribute_SuppliesExtraValues(int x, int y, int z)
    {
        await Assert.That(x).IsEqualTo(1337);
        await Assert.That(y).IsNotEqualTo(0);
        await Assert.That(z).IsEqualTo(42);
    }

    public class MemberHost
    {
        public static IEnumerable<string> Strings
        {
            get
            {
                yield return "one";
                yield return "two";
            }
        }

        public static IEnumerable<object[]> ObjectArrays
        {
            get
            {
                yield return ["hello", 1, new RecordType<string>("world")];
                yield return ["foo", 2, new RecordType<string>("bar")];
            }
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
    }

    public class MyCustomAutoMemberDataSourceAttribute<T> : AutoMemberDataSourceAttribute<T>
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
