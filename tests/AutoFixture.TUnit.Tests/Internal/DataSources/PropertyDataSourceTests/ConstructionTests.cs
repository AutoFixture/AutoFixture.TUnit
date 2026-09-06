using AutoFixture.TUnit.Internal;
using AutoFixture.TUnit.Tests.Internal.DataGeneratorMetadataSupport;
using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;
using PropertyDataSource = AutoFixture.TUnit.Internal.PropertyDataSource;

namespace AutoFixture.TUnit.Tests.Internal.DataSources.PropertyDataSourceTests;

public class PropertyDataSourceTests
{
    [Test]
    public async Task Constructor_WhenCreated_IsDataSource()
    {
        // Arrange
        var sourceProperty = typeof(PropertyDataSourceTests)
            .GetProperty(nameof(TestDataPropertyWithMixedValues));
        var sut = new PropertyDataSource(sourceProperty);

        // Assert
        await Assert.That(sut).IsAssignableTo<IDataSource>();
    }

    public static IEnumerable<object[]> TestDataPropertyWithMixedValues =>
    [
        ["hello", 1, new PropertyHolder<string> { Property = "world" }],
        ["foo", 2, new PropertyHolder<string> { Property = "bar" }],
        ["Han", 3, new PropertyHolder<string> { Property = "Solo" }]
    ];

    public static object NonEnumerableProperty => new object();

    [Test]
    public async Task Constructor_WhenPropertyIsNull_Throws()
    {
        // Act & Assert
        await Assert.That(() => new PropertyDataSource(null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Property_WhenRead_ReturnsConstructorArgument()
    {
        // Arrange
        var expected = typeof(PropertyDataSourceTests)
            .GetProperty(nameof(TestDataPropertyWithMixedValues));
        var sut = new PropertyDataSource(expected);

        // Act
        var result = sut.PropertyInfo;

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task GetData_WhenTestMethodIsNull_Throws()
    {
        // Arrange
        var sourceProperty = typeof(PropertyDataSourceTests)
            .GetProperty(nameof(TestDataPropertyWithMixedValues));
        var sut = new PropertyDataSource(sourceProperty);

        // Act & Assert
        await Assert.That(() => sut.GenerateDataSources(null).ToArray()).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task GetData_WhenSourceIsScalar_ReturnsSingleRow()
    {
        // Arrange
        var sourceProperty = typeof(PropertyDataSourceTests)
            .GetProperty(nameof(NonEnumerableProperty));
        var sut = new PropertyDataSource(sourceProperty);
        var method = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithReferenceTypeParameter));

        // Act
        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(method))
            .Select(x => x())
            .ToArray();

        // Assert
        await Assert.That(result.Length).IsEqualTo(1);
        await Assert.That(result[0].Length).IsEqualTo(1);
    }

    [Test]
    public async Task GetData_WhenCalled_ReturnsDataMatchingParameters()
    {
        // Arrange
        var expected = new[]
        {
            new object[] { "hello", 1, new RecordType<string>("world") },
            new object[] { "foo", 2, new RecordType<string>("bar") },
            new object[] { "Han", 3, new RecordType<string>("Solo") }
        };
        var sourceProperty = typeof(PropertyDataSourceTests)
            .GetProperty(nameof(TestDataPropertyWithRecordValues));
        var sut = new PropertyDataSource(sourceProperty);
        var method = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithRecordTypeParameter));

        // Act
        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(method))
            .Select(x => x())
            .ToArray();

        // Assert
        await Assert.That(result).IsEquivalentTo(expected);
    }

    public static IEnumerable<object[]> TestDataPropertyWithRecordValues =>
    [
        ["hello", 1, new RecordType<string>("world")],
        ["foo", 2, new RecordType<string>("bar")],
        ["Han", 3, new RecordType<string>("Solo")]
    ];

    [Test]
    public async Task GetData_WhenSourceReturnsNull_ReturnsNullArguments()
    {
        // Arrange
        var expected = new[]
        {
            new object[] { null, 1, null },
            new object[] { null, 2, null },
            new object[] { null, 3, null }
        };
        var sourceProperty = typeof(PropertyDataSourceTests)
            .GetProperty(nameof(TestDataPropertyWithNullValues));
        var sut = new PropertyDataSource(sourceProperty);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithRecordTypeParameter));

        // Act
        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            .Select(x => x())
            .ToArray();

        // Assert
        await Assert.That(result).IsEquivalentTo(expected);
    }

    public static IEnumerable<object[]> TestDataPropertyWithNullValues =>
    [
        [null, 1, null],
        [null, 2, null],
        [null, 3, null]
    ];
}
