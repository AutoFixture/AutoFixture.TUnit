namespace AutoFixture.TUnit.Tests.Attributes.Customizations;

public class FrozenAttributeTest
{
    [Test]
    public async Task Constructor_WhenCreated_IsAttribute()
    {
        // Arrange
        // Act
        var sut = new FrozenAttribute();
        // Assert
        await Assert.That(sut).IsAssignableTo<CustomizeAttribute>();
    }

    [Test]
    public async Task GetCustomization_WhenParameterIsNull_Throws()
    {
        // Arrange
        var sut = new FrozenAttribute();
        // Act & assert
        await Assert.That(() => sut.GetCustomization(null))
            .ThrowsExactly<ArgumentNullException>();
    }
}
