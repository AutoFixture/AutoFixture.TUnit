using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests.Attributes.Customizations;

public class NoAutoPropertiesAttributeTest
{
    [Test]
    public async Task Constructor_WhenCreated_IsAttribute()
    {
        // Arrange
        // Act
        var sut = new NoAutoPropertiesAttribute();
        // Assert
        await Assert.That(sut).IsAssignableTo<CustomizeAttribute>();
    }

    [Test]
    public async Task GetCustomization_WhenParameterIsNull_Throws()
    {
        // Arrange
        var sut = new NoAutoPropertiesAttribute();
        // Act & assert
        await Assert.That(() => sut.GetCustomization(null))
            .ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task GetCustomization_WhenParameterProvided_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new NoAutoPropertiesAttribute();
        var parameter = TypeWithOverloadedMembers
            .GetDoSomethingMethod(typeof(object))
            .GetParameters().Single();
        // Act
        var result = sut.GetCustomization(parameter);
        // Assert
        await Assert.That(result).IsAssignableTo<NoAutoPropertiesCustomization>();
    }
}
