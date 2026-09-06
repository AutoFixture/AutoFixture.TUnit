using AutoFixture.TUnit.Internal;
using AutoFixture.TUnit.Tests.Internal.DataGeneratorMetadataSupport;
using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests.Internal.DataSources.InlineDataSourceTests;

public class InlineDataSourceTests
{
    [Test]
    public async Task Constructor_WhenCreated_IsDataSource()
    {
        // Arrange
        // Act
        var sut = new InlineDataSource([
        ]);
        // Assert
        await Assert.That(sut).IsAssignableTo<IDataSource>();
    }

    [Test]
    public async Task Constructor_WhenValuesIsNull_Throws()
    {
        // Arrange
        // Act & Assert
        await Assert.That(() =>
            new InlineDataSource(null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Values_WhenRead_ReturnsConstructorArguments()
    {
        // Arrange
        var expectedValues = Array.Empty<object>();
        var sut = new InlineDataSource(expectedValues);
        // Act
        var result = sut.Values;
        // Assert
        await Assert.That(result).IsEquivalentTo(expectedValues);
    }

    [Test]
    public async Task GetData_WhenMethodIsNull_Throws()
    {
        // Arrange
        var sut = new InlineDataSource([
        ]);
        // Act & Assert - call GetData directly so InlineDataSource's null guard is hit
        await Assert.That(async () =>
        {
            await foreach (var _ in sut.GetData(null))
            {
            }
        }).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task GetData_WhenArgumentCountExceedsParameterCount_Throws()
    {
        // Arrange
        var values = new object[] { "aloha", 42, 12.3d, "extra" };
        var sut = new InlineDataSource(values);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));

        // Act & Assert
        await Assert.That(() =>
            sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
                .Select(x => x()).ToArray())
        .ThrowsExactly<InvalidOperationException>();
    }

    [Test]
    public async Task GetData_WhenArgumentCountMatchesParameterCount_ReturnsTestData()
    {
        // Arrange
        var values = new object[] { "aloha", 42, 12.3d };
        var sut = new InlineDataSource(values);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));

        // Act
        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod));

        // Assert
        var testData = await Assert.That(result).HasSingleItem();
        await Assert.That(testData).IsEqualTo(values);
    }

    [Test]
    public async Task GetData_WhenArgumentCountLessThanParameterCount_ReturnsAllArguments()
    {
        // Arrange
        var values = new object[] { "aloha", 42 };
        var sut = new InlineDataSource(values);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));

        // Act
        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod));

        // Assert
        var testData = await Assert.That(result).HasSingleItem();
        await Assert.That(testData).IsEqualTo(values);
    }
}
