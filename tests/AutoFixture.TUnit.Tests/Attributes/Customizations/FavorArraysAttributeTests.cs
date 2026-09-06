using AutoFixture.Kernel;
using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests.Attributes.Customizations;

public class FavorArraysAttributeTest
{
    [Test]
    public async Task Constructor_WhenCreated_IsAttribute()
    {
        // Arrange
        // Act
        var sut = new FavorArraysAttribute();
        // Assert
        await Assert.That(sut).IsAssignableTo<CustomizeAttribute>();
    }

    [Test]
    public async Task GetCustomization_WhenParameterIsNull_Throws()
    {
        // Arrange
        var sut = new FavorArraysAttribute();
        // Act & assert
        await Assert.That(() =>
            sut.GetCustomization(null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task GetCustomization_WhenParameterProvided_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new FavorArraysAttribute();
        var parameter = typeof(TypeWithOverloadedMembers)
            .GetMethod(nameof(TypeWithOverloadedMembers.DoSomething), [typeof(object)])
            .GetParameters().Single();
        // Act
        var result = sut.GetCustomization(parameter);
        // Assert
        await Assert.That(result).IsAssignableTo<ConstructorCustomization>();
        var invoker = (ConstructorCustomization)result;
        await Assert.That(invoker.TargetType).IsEqualTo(parameter.ParameterType);
        await Assert.That(invoker.Query).IsAssignableTo<ArrayFavoringConstructorQuery>();
    }
}
