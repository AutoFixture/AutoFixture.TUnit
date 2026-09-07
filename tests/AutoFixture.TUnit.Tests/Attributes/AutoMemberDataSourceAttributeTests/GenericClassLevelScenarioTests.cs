using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;
using ConcreteType = TestTypeFoundation.ConcreteType;

namespace AutoFixture.TUnit.Tests.Attributes.AutoMemberDataSourceAttributeTests;

/// <summary>
/// Class-level [AutoMemberDataSource&lt;T&gt;] scenarios parallel to the non-generic class-level set.
/// </summary>
[AutoMemberDataSource<GenericMemberHost>(nameof(GenericMemberHost.Names))]
public class GenericClassLevelPrimitiveScenarioTests(string name, int count)
{
    [Test]
    public async Task WhenGenericAppliedToClass_UsesMemberValueAndFillsRemaining()
    {
        await Assert.That<string[]>(["one", "two"]).Contains(name);
        await Assert.That(count).IsNotEqualTo(0);
    }
}

[AutoMemberDataSource<GenericMemberHost>(nameof(GenericMemberHost.Names))]
public class GenericClassLevelComplexTypeScenarioTests(
    string name,
    PropertyHolder<Version> ph,
    SingleParameterType<ConcreteType> spt,
    MyClass sut)
{
    [Test]
    public async Task WhenGenericAppliedToClass_UsesMemberValueAndFillsComplexRemaining()
    {
        await Assert.That<string[]>(["one", "two"]).Contains(name);

        await Assert.That(ph).IsNotNull();
        await Assert.That(ph.Property).IsNotNull();

        await Assert.That(spt).IsNotNull();
        await Assert.That(spt.Parameter).IsNotNull();

        await Assert.That(sut).IsNotNull();
        await Assert.That(sut.Echo(7)).IsEqualTo(7);
    }
}

[AutoMemberDataSource<GenericClassLevelFrozenScenarioTests>(nameof(OneEmptyRow))]
public class GenericClassLevelFrozenScenarioTests([Frozen] Guid first, Guid second)
{
    public static IEnumerable<object[]> OneEmptyRow()
    {
        yield return [];
    }

    [Test]
    public async Task WhenGenericFrozenOnConstructorParameter_SharesInstanceWithLaterParameter()
    {
        await Assert.That(second).IsEqualTo(first);
    }
}

public class GenericMemberHost
{
    public static IEnumerable<string> Names
    {
        get
        {
            yield return "one";
            yield return "two";
        }
    }
}
