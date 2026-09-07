using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;
using ConcreteType = TestTypeFoundation.ConcreteType;

namespace AutoFixture.TUnit.Tests.Attributes.AutoMemberDataSourceAttributeTests;

/// <summary>
/// End-to-end [AutoMemberDataSource] on the test class (TUnit ClassParameters).
/// Ordered simple → complex; mirrors AutoDataSource class-level scenarios.
/// </summary>
[AutoMemberDataSource(nameof(Names))]
public class ClassLevelPrimitiveScenarioTests(string name, int count)
{
    public static IEnumerable<string> Names => ["one", "two"];

    [Test]
    public async Task WhenAppliedToClass_UsesMemberValueAndFillsRemaining()
    {
        await Assert.That<string[]>(["one", "two"]).Contains(name);
        await Assert.That(count).IsNotEqualTo(0);
    }
}

[AutoMemberDataSource(nameof(Names))]
public class ClassLevelComplexTypeScenarioTests(
    string name,
    PropertyHolder<Version> ph,
    SingleParameterType<ConcreteType> spt,
    MyClass sut)
{
    public static IEnumerable<string> Names => ["alpha", "beta"];

    [Test]
    public async Task WhenAppliedToClass_UsesMemberValueAndFillsComplexRemaining()
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

[AutoMemberDataSource(nameof(OneEmptyRow))]
public class ClassLevelFrozenScenarioTests([Frozen] Guid first, Guid second)
{
    public static IEnumerable<object[]> OneEmptyRow()
    {
        yield return [];
    }

    [Test]
    public async Task WhenFrozenOnConstructorParameter_SharesInstanceWithLaterParameter()
    {
        await Assert.That(second).IsEqualTo(first);
    }
}

[AutoMemberDataSource(nameof(OneEmptyRow))]
public class ClassLevelModestScenarioTests([Modest] MultiUnorderedConstructorType value)
{
    public static IEnumerable<object[]> OneEmptyRow()
    {
        yield return [];
    }

    [Test]
    public async Task WhenModestOnConstructorParameter_UsesModestConstructor()
    {
        await Assert.That(string.IsNullOrEmpty(value.Text)).IsTrue();
        await Assert.That(value.Number).IsEqualTo(0);
    }
}

[AutoMemberDataSource(nameof(Names))]
public class ClassLevelMultiConstructorParameterizedFirstScenarioTests
{
    public static IEnumerable<string> Names => ["from-member"];

    private readonly string name;

    public ClassLevelMultiConstructorParameterizedFirstScenarioTests(string name)
    {
        this.name = name;
    }

    public ClassLevelMultiConstructorParameterizedFirstScenarioTests()
    {
        this.name = "parameterless";
    }

    [Test]
    public async Task WhenParameterizedConstructorDeclaredFirst_UsesMemberConstructorArgument()
    {
        await Assert.That(this.name).IsEqualTo("from-member");
    }
}

[AutoMemberDataSource(nameof(Counts))]
public class ClassLevelMultiConstructorIntFirstScenarioTests
{
    public static IEnumerable<int> Counts => [11, 22];

    private readonly string name;
    private readonly int count;

    public ClassLevelMultiConstructorIntFirstScenarioTests(int count)
    {
        this.name = "int-ctor";
        this.count = count;
    }

    public ClassLevelMultiConstructorIntFirstScenarioTests(string name)
    {
        this.name = name;
        this.count = -1;
    }

    [Test]
    public async Task WhenIntConstructorDeclaredFirst_UsesMemberIntArgument()
    {
        await Assert.That(this.name).IsEqualTo("int-ctor");
        await Assert.That<int[]>([11, 22]).Contains(this.count);
    }
}
