using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests;

public class CustomizeAttributeTest
{
    [Test]
    public async Task TestableSutIsSut()
    {
        // Arrange
        // Act
        var sut = new DelegatingCustomizeAttribute();
        // Assert
        await Assert.That(sut).IsAssignableTo<CustomizeAttribute>();
    }

    [Test]
    public async Task SutIsAttribute()
    {
        // Arrange
        // Act
        var sut = new DelegatingCustomizeAttribute();
        // Assert
        await Assert.That(sut).IsAssignableTo<Attribute>();
    }

    [Test]
    public async Task SutImplementsIParameterCustomizationSource()
    {
        // Arrange
        // Act
        var sut = new DelegatingCustomizeAttribute();
        // Assert
        await Assert.That(sut).IsAssignableTo<IParameterCustomizationSource>();
    }
}