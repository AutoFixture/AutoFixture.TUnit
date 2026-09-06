using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;
using ConcreteType = TestTypeFoundation.ConcreteType;

namespace AutoFixture.TUnit.Tests.Attributes.CompositeDataSourceAttributeTests;

/// <summary>
/// End-to-end [CompositeDataSource] on the test class (TUnit ClassParameters).
/// Ordered simple → complex; mirrors AutoDataSource class-level scenarios.
/// </summary>
[ClassLevelPrimitiveComposite]
public class ClassLevelPrimitiveScenarioTests(string name, int count)
{
    [Test]
    public async Task WhenAppliedToClass_UsesCompositeValues()
    {
        await Assert.That(name).IsEqualTo("seed");
        await Assert.That(count).IsEqualTo(7);
    }
}

[ClassLevelComplexComposite]
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

[ClassLevelAutoDataComposite]
public class ClassLevelFrozenScenarioTests([Frozen] Guid first, Guid second)
{
    [Test]
    public async Task WhenFrozenOnConstructorParameter_SharesInstanceWithLaterParameter()
    {
        await Assert.That(second).IsEqualTo(first);
    }
}

[ClassLevelAutoDataComposite]
public class ClassLevelModestScenarioTests([Modest] MultiUnorderedConstructorType value)
{
    [Test]
    public async Task WhenModestOnConstructorParameter_UsesModestConstructor()
    {
        await Assert.That(string.IsNullOrEmpty(value.Text)).IsTrue();
        await Assert.That(value.Number).IsEqualTo(0);
    }
}

[ClassLevelInlineNameComposite]
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
    public async Task WhenParameterizedConstructorDeclaredFirst_UsesCompositeConstructorArgument()
    {
        await Assert.That(this.name).IsEqualTo("from-composite");
    }
}

[ClassLevelInlineIntComposite]
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
    public async Task WhenIntConstructorDeclaredFirst_UsesCompositeIntArgument()
    {
        await Assert.That(this.name).IsEqualTo("int-ctor");
        await Assert.That(this.count).IsEqualTo(42);
    }
}

public sealed class ClassLevelPrimitiveCompositeAttribute : CompositeDataSourceAttribute
{
    public ClassLevelPrimitiveCompositeAttribute()
        : base(
            new FixedRowsAttribute(["seed"]),
            new FixedRowsAttribute([null!, 7]))
    {
    }
}

public sealed class ClassLevelComplexCompositeAttribute : CompositeDataSourceAttribute
{
    public ClassLevelComplexCompositeAttribute()
        : base(
            new AutoArgumentsAttribute("seed"),
            new AutoDataSourceAttribute())
    {
    }
}

public sealed class ClassLevelAutoDataCompositeAttribute : CompositeDataSourceAttribute
{
    public ClassLevelAutoDataCompositeAttribute()
        : base(new AutoDataSourceAttribute())
    {
    }
}

public sealed class ClassLevelInlineNameCompositeAttribute : CompositeDataSourceAttribute
{
    public ClassLevelInlineNameCompositeAttribute()
        : base(
            new AutoArgumentsAttribute("from-composite"),
            new FixedRowsAttribute(["ignored"]))
    {
    }
}

public sealed class ClassLevelInlineIntCompositeAttribute : CompositeDataSourceAttribute
{
    public ClassLevelInlineIntCompositeAttribute()
        : base(
            new AutoArgumentsAttribute(42),
            new FixedRowsAttribute([0]))
    {
    }
}

/// <summary>
/// Yields fixed rows without AutoFixture fill — same public extension model as UsageScenarioTests.
/// </summary>
public sealed class FixedRowsAttribute : BaseDataSourceAttribute
{
    private readonly object?[][] rows;

    public FixedRowsAttribute(params object?[][] rows)
    {
        this.rows = rows;
    }

#pragma warning disable CS1998
    public override async IAsyncEnumerable<Func<Task<object?[]?>>> GetData(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        foreach (var row in this.rows)
        {
            var captured = row;
            yield return () => Task.FromResult<object?[]?>(captured);
        }
    }
#pragma warning restore CS1998
}
