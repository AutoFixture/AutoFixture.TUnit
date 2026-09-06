using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;
using ConcreteType = TestTypeFoundation.ConcreteType;

namespace AutoFixture.TUnit.Tests.Attributes.AutoDataSourceAttributeTests;

/// <summary>
/// End-to-end [AutoDataSource] on the test class (TUnit ClassParameters).
/// Ordered simple → complex.
/// </summary>
[AutoDataSource]
public class ClassLevelPrimitiveScenarioTests(string name, int count)
{
    [Test]
    public async Task WhenAppliedToClass_FillsPrimitiveConstructorParameters()
    {
        await Assert.That(name).IsNotEmpty();
        await Assert.That(count).IsNotEqualTo(0);
    }
}

[AutoDataSource]
public class ClassLevelComplexTypeScenarioTests(
    PropertyHolder<Version> ph,
    SingleParameterType<ConcreteType> spt,
    MyClass sut)
{
    [Test]
    public async Task WhenAppliedToClass_FillsComplexConstructorParameters()
    {
        await Assert.That(ph).IsNotNull();
        await Assert.That(ph.Property).IsNotNull();

        await Assert.That(spt).IsNotNull();
        await Assert.That(spt.Parameter).IsNotNull();

        await Assert.That(sut).IsNotNull();
        await Assert.That(sut.Echo(7)).IsEqualTo(7);
    }
}

[AutoDataSource]
public class ClassLevelFrozenScenarioTests([Frozen] Guid first, Guid second)
{
    [Test]
    public async Task WhenFrozenOnConstructorParameter_SharesInstanceWithLaterParameter()
    {
        await Assert.That(second).IsEqualTo(first);
    }
}

[AutoDataSource]
public class ClassLevelModestScenarioTests([Modest] MultiUnorderedConstructorType value)
{
    [Test]
    public async Task WhenModestOnConstructorParameter_UsesModestConstructor()
    {
        await Assert.That(string.IsNullOrEmpty(value.Text)).IsTrue();
        await Assert.That(value.Number).IsEqualTo(0);
    }
}

/// <summary>
/// Parameterized constructor is declared first so TUnit Class.Parameters describe it.
/// A parameterless overload exists so GetConstructors().First() is not a reliable match.
/// </summary>
[AutoDataSource]
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
    public async Task WhenParameterizedConstructorDeclaredFirst_FillsMatchingConstructor()
    {
        await Assert.That(this.name).IsNotEqualTo("parameterless");
        await Assert.That(this.name).IsNotEmpty();
    }
}

/// <summary>
/// Two parameterized constructors: int overload is declared first, so TUnit Class.Parameters
/// describe that constructor and AutoDataSource fills it.
/// </summary>
[AutoDataSource]
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
    public async Task WhenIntConstructorDeclaredFirst_FillsIntConstructor()
    {
        await Assert.That(this.name).IsEqualTo("int-ctor");
        await Assert.That(this.count).IsNotEqualTo(0);
        await Assert.That(this.count).IsNotEqualTo(-1);
    }
}
