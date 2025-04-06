namespace AutoFixture.TUnit.Tests;

public class UnitTest1
{
    [Test]
    public async Task Test1()
    {
        // Arrange
        var sut = new Class1();

        // Assert
        await Assert.That(sut).IsNotNull();
    }
}