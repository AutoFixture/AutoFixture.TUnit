using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests.Attributes.Customizations;

public class CustomizeAttributeTest
{
    [Test]
    public async Task Constructor_WhenCreated_IsCustomizeAttribute()
    {
        // Arrange && Act
        var sut = new DelegatingCustomizeAttribute();

        // Assert
        await Assert.That(sut).IsAssignableTo<CustomizeAttribute>();
    }

    [Test]
    public async Task Constructor_WhenCreated_IsAttribute()
    {
        // Arrange && Act
        var sut = new DelegatingCustomizeAttribute();

        // Assert
        await Assert.That(sut).IsAssignableTo<Attribute>();
    }

    [Test]
    public async Task Constructor_WhenCreated_ImplementsIParameterCustomizationSource()
    {
        // Arrange && Act
        var sut = new DelegatingCustomizeAttribute();

        // Assert
        await Assert.That(sut).IsAssignableTo<IParameterCustomizationSource>();
    }
}
