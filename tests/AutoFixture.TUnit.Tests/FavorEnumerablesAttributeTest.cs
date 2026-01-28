using AutoFixture.Kernel;
using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests;

public class FavorEnumerablesAttributeTest
{
    [Test]
    public async Task SutIsAttribute()
    {
        // Arrange
        // Act
        var sut = new FavorEnumerablesAttribute();
        // Assert
        await Assert.That(sut).IsAssignableTo<CustomizeAttribute>();
    }

    [Test]
    public async Task GetCustomizationFromNullParameterThrows()
    {
        // Arrange
        var sut = new FavorEnumerablesAttribute();
        // Act & assert
        await Assert.That(() =>
            sut.GetCustomization(null!)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task GetCustomizationReturnsCorrectResult()
    {
        // Arrange
        var sut = new FavorEnumerablesAttribute();
        var parameter = typeof(TypeWithOverloadedMembers)
            .GetMethod(nameof(TypeWithOverloadedMembers.DoSomething), [typeof(object)])!
            .GetParameters().Single();
        // Act
        var result = sut.GetCustomization(parameter);
        // Assert
        await Assert.That(result).IsAssignableTo<ConstructorCustomization>();
        var invoker = (ConstructorCustomization)result;
        await Assert.That(invoker.TargetType).IsEqualTo(parameter.ParameterType);
        await Assert.That(invoker.Query).IsAssignableTo<EnumerableFavoringConstructorQuery>();
    }
}