using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;
using ConcreteType = TestTypeFoundation.ConcreteType;

namespace AutoFixture.TUnit.Tests.Attributes.AutoDataSourceAttributeTests;

public class ScenarioTests
{
    [Test, AutoDataSource]
    public async Task WhenIntegerParameter_ProvidesNonDefaultValue(int primitiveValue)
    {
        await Assert.That(primitiveValue).IsNotEqualTo(0);
    }

    [Test, AutoDataSource]
    public async Task WhenStringParameter_ProvidesNonDefaultValue(string text)
    {
        await Assert.That(text).StartsWith("text");
    }

    [Test, AutoDataSource]
    public async Task WhenObjectParameter_ProvidesNonNullInstance(PropertyHolder<Version> ph)
    {
        await Assert.That(ph).IsNotNull();
        await Assert.That(ph.Property).IsNotNull();
    }

    [Test, AutoDataSource]
    public async Task WhenMultipleParameters_ProvidesNonNullInstances(PropertyHolder<Version> ph, SingleParameterType<ConcreteType> spt)
    {
        await Assert.That(ph).IsNotNull();
        await Assert.That(ph.Property).IsNotNull();

        await Assert.That(spt).IsNotNull();
        await Assert.That(spt.Parameter).IsNotNull();
    }

    [Test, AutoDataSource]
    public async Task WhenExpectedNumberAndSut_AssignsExpectedNumberToSut(int expectedNumber, MyClass sut)
    {
        int result = sut.Echo(expectedNumber);

        await Assert.That(result).IsEqualTo(expectedNumber);
    }
}
