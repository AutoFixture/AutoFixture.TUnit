using System.Reflection;
using AutoFixture.TUnit.Tests.Internal.DataGeneratorMetadataSupport;
using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests.Attributes.CompositeDataSourceAttributeTests;

public class CompositeDataSourceAttributeTest
{
    [Test]
    public async Task Constructor_WhenCreated_IsBaseDataSourceAttribute()
    {
        // Arrange & Act
        var sut = new CompositeDataSourceAttribute();

        // Assert
        await Assert.That(sut).IsAssignableTo<BaseDataSourceAttribute>();
    }

    [Test]
    public async Task Constructor_WhenArrayIsNull_Throws()
    {
        // Arrange
        // Act & assert
        await Assert.That(() => new CompositeDataSourceAttribute(null))
            .ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Attributes_WhenInitializedWithArray_ReturnsSameAttributes()
    {
        // Arrange
        var a = () => { };
        var method = a.GetMethodInfo();

        var attributes = new BaseDataSourceAttribute[]
        {
            new FakeDataAttribute(method, []),
            new FakeDataAttribute(method, []),
            new FakeDataAttribute(method, [])
        };

        var sut = new CompositeDataSourceAttribute(attributes);
        // Act
        IEnumerable<BaseDataSourceAttribute> result = sut.Attributes;
        // Assert
        await Assert.That(result).IsEquivalentTo(attributes);
    }

    [Test]
    public void Constructor_WhenEnumerableIsNull_Throws()
    {
        // Act & assert
        Assert.Throws<ArgumentNullException>(
            () => _ = new CompositeDataSourceAttribute(((IEnumerable<BaseDataSourceAttribute>)null)));
    }

    [Test]
    public async Task Attributes_WhenInitializedWithEnumerable_ReturnsSameAttributes()
    {
        // Arrange
        var a = () => { };
        var method = a.GetMethodInfo();

        // Use a List so the IEnumerable ctor cannot cast to array and must call ToArray().
        IEnumerable<BaseDataSourceAttribute> attributes = new List<BaseDataSourceAttribute>
        {
            new FakeDataAttribute(method, []),
            new FakeDataAttribute(method, []),
            new FakeDataAttribute(method, [])
        };

        var sut = new CompositeDataSourceAttribute(attributes);
        // Act
        var result = sut.Attributes;
        // Assert
        await Assert.That(result).IsEquivalentTo(attributes);
    }

    [Test]
    public async Task GetData_WhenGeneratorMetadataIsNull_Throws()
    {
        // Arrange
        var sut = new CompositeDataSourceAttribute();

        // Act & assert - call GetData directly so the attribute's own null guard is hit
        await Assert.That(async () =>
        {
            await foreach (var _ in sut.GetData(null))
            {
            }
        }).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task GetData_WhenMethodHasNoParameters_ReturnsNoRows()
    {
        // Arrange
        var a = () => { };
        var method = a.GetMethodInfo();
        var sut = new CompositeDataSourceAttribute(
            new FakeDataAttribute(method, []),
            new FakeDataAttribute(method, []),
            new FakeDataAttribute(method, []));
        var dataGeneratorMetadata = DataGeneratorMetadataHelper
            .CreateDataGeneratorMetadata(method);

        // Act
        var result = sut.GenerateDataSources(dataGeneratorMetadata)
                .Select(x => x()).ToArray();

        // Assert
        await Assert.That(result).All().Satisfy(row => row.IsEmpty());
    }
}
