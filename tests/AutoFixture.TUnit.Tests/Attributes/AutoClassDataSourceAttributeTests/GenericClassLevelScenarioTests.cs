using AutoFixture.TUnit.Tests.TestTypes;
using AutoFixture.TUnit.Tests.TestTypes.DataSourceFixtures;
using TestTypeFoundation;
using ConcreteType = TestTypeFoundation.ConcreteType;

namespace AutoFixture.TUnit.Tests.Attributes.AutoClassDataSourceAttributeTests;

/// <summary>
/// Class-level [AutoClassDataSource&lt;T&gt;] scenarios parallel to the non-generic class-level set.
/// </summary>
[AutoClassDataSource<StringSequenceClassData>]
public class GenericClassLevelPrimitiveScenarioTests(string name, int count)
{
    [Test]
    public async Task WhenGenericAppliedToClass_UsesClassValueAndFillsRemaining()
    {
        await Assert.That<string[]>(["alpha", "beta"]).Contains(name);
        await Assert.That(count).IsNotEqualTo(0);
    }
}

[AutoClassDataSource<StringSequenceClassData>]
public class GenericClassLevelComplexTypeScenarioTests(
    string name,
    PropertyHolder<Version> ph,
    SingleParameterType<ConcreteType> spt,
    MyClass sut)
{
    [Test]
    public async Task WhenGenericAppliedToClass_UsesClassValueAndFillsComplexRemaining()
    {
        await Assert.That<string[]>(["alpha", "beta"]).Contains(name);

        await Assert.That(ph).IsNotNull();
        await Assert.That(ph.Property).IsNotNull();

        await Assert.That(spt).IsNotNull();
        await Assert.That(spt.Parameter).IsNotNull();

        await Assert.That(sut).IsNotNull();
        await Assert.That(sut.Echo(7)).IsEqualTo(7);
    }
}

[AutoClassDataSource<OneEmptyRowClassData>]
public class GenericClassLevelFrozenScenarioTests([Frozen] Guid first, Guid second)
{
    [Test]
    public async Task WhenGenericFrozenOnConstructorParameter_SharesInstanceWithLaterParameter()
    {
        await Assert.That(second).IsEqualTo(first);
    }
}
