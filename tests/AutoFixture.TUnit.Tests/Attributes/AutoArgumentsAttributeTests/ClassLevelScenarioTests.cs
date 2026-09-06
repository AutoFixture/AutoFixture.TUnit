using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;
using ConcreteType = TestTypeFoundation.ConcreteType;

namespace AutoFixture.TUnit.Tests.Attributes.AutoArgumentsAttributeTests;

/// <summary>
/// End-to-end [AutoArguments] on the test class (TUnit ClassParameters).
/// Ordered simple → complex; mirrors AutoDataSource class-level scenarios.
/// </summary>
[AutoArguments("seed")]
public class ClassLevelPrimitiveScenarioTests(string name, int count)
{
    [Test]
    public async Task WhenAppliedToClass_UsesInlineValueAndFillsRemaining()
    {
        await Assert.That(name).IsEqualTo("seed");
        await Assert.That(count).IsNotEqualTo(0);
    }
}

[AutoArguments("seed")]
public class ClassLevelComplexTypeScenarioTests(
    string name,
    PropertyHolder<Version> ph,
    SingleParameterType<ConcreteType> spt,
    MyClass sut)
{
    [Test]
    public async Task WhenAppliedToClass_UsesInlineValueAndFillsComplexRemaining()
    {
        await Assert.That(name).IsEqualTo("seed");

        await Assert.That(ph).IsNotNull();
        await Assert.That(ph.Property).IsNotNull();

        await Assert.That(spt).IsNotNull();
        await Assert.That(spt.Parameter).IsNotNull();

        await Assert.That(sut).IsNotNull();
        await Assert.That(sut.Echo(7)).IsEqualTo(7);
    }
}

[AutoArguments]
public class ClassLevelFrozenScenarioTests([Frozen] Guid first, Guid second)
{
    [Test]
    public async Task WhenFrozenOnConstructorParameter_SharesInstanceWithLaterParameter()
    {
        await Assert.That(second).IsEqualTo(first);
    }
}

[AutoArguments]
public class ClassLevelModestScenarioTests([Modest] MultiUnorderedConstructorType value)
{
    [Test]
    public async Task WhenModestOnConstructorParameter_UsesModestConstructor()
    {
        await Assert.That(string.IsNullOrEmpty(value.Text)).IsTrue();
        await Assert.That(value.Number).IsEqualTo(0);
    }
}

[AutoArguments("from-inline")]
public class ClassLevelMultiConstructorParameterizedFirstScenarioTests
{
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
    public async Task WhenParameterizedConstructorDeclaredFirst_UsesInlineConstructorArgument()
    {
        await Assert.That(this.name).IsEqualTo("from-inline");
    }
}

[AutoArguments(42)]
public class ClassLevelMultiConstructorIntFirstScenarioTests
{
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
    public async Task WhenIntConstructorDeclaredFirst_UsesInlineIntArgument()
    {
        await Assert.That(this.name).IsEqualTo("int-ctor");
        await Assert.That(this.count).IsEqualTo(42);
    }
}
