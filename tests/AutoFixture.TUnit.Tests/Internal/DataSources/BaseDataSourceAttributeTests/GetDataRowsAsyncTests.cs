using AutoFixture.TUnit.Internal;
using AutoFixture.TUnit.Tests.Internal.DataGeneratorMetadataSupport;
using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests.Internal.DataSources.BaseDataSourceAttributeTests;

public class DataSourceTests
{
    [Test]
    public async Task Constructor_WhenCreated_IsDataSource()
    {
        // Arrange
        var sut = new DelegatingDataSource();

        // Assert
        await Assert.That(sut).IsAssignableTo<IDataSource>();
    }

    [Test]
    public async Task GetDataRowsAsync_WhenMethodHasNoParameters_ReturnsSingleEmptyArray()
    {
        // Arrange
        var sut = new DelegatingDataSource();
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithoutParameters));

        // Act
        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod)).ToArray();

        // Assert
        await Assert.That(result.Length).IsEqualTo(1);
        var item = result.Single()();

        await Assert.That(item).IsEmpty();
    }

    [Test]
    public async Task GetDataRowsAsync_WhenNoDataFound_Throws()
    {
        // Arrange
        var sut = new DelegatingDataSource { TestData = null };
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithSingleParameter));

        // Act & Assert
        await Assert.That(() => sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod)).ToArray()).ThrowsExactly<InvalidOperationException>();
    }

    [Test]
    public async Task GetDataRowsAsync_WhenMethodHasSingleParameter_ReturnsSingleItemArray()
    {
        // Arrange
        var sut = new DelegatingDataSource
        {
            TestData = [["hello"]]
        };
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithSingleParameter));

        // Act
        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod)).ToArray();

        // Assert
        await Assert.That(result.Length).IsEqualTo(1);
        var testData = result.Single()();
        await Assert.That(testData.Length).IsEqualTo(1);
        await Assert.That((string)testData[0]).IsEqualTo("hello");
    }

    [Test]
    public async Task GetDataRowsAsync_WhenCalled_ReturnsArgumentsFittingParameters()
    {
        // Arrange
        object[][] testData = [
            [ "hello", 16, 32.86d ],
            [null, -1, -20.22],
            ["one", 2],
            [null],
            [],
        ];
        var sut = new DelegatingDataSource { TestData = testData };
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));

        // Act
        var actual = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            .Select(x => x())
            .ToArray();

        // Assert
        await Assert.That(actual.Length).IsEqualTo(testData.Length);

        foreach (var a in actual)
        {
            await Assert.That(a.Length).IsBetween(0, 3);
        }
    }

    [Test]
    public async Task GetDataRowsAsync_WhenMoreArgumentsThanParameters_Throws()
    {
        // Arrange
        object[][] testData = [[ "hello", 16, 32.86d, "extra" ]];
        var sut = new DelegatingDataSource { TestData = testData };
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));

        // Act & Assert
        await Assert.That(() => sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod)).ToArray()).ThrowsExactly<InvalidOperationException>();
    }
}
