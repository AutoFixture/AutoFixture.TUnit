using System.Reflection;
using AutoFixture.TUnit.Tests.TestTypes;
using TUnit.Assertions.AssertConditions.Throws;

namespace AutoFixture.TUnit.Tests;

public class CompositeDataSourceAttributeTest
{
    [Test]
    public async Task SutIsDataAttribute()
    {
        // Arrange & Act
        var sut = new CompositeDataSourceAttribute();

        // Assert
        await Assert.That(sut).IsAssignableTo<BaseDataSourceAttribute>();
    }

    [Test]
    public async Task InitializeWithNullArrayThrows()
    {
        // Arrange
        // Act & assert
        await Assert.That(() => new CompositeDataSourceAttribute(null!))
            .ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task AttributesIsCorrectWhenInitializedWithArray()
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
    public void InitializeWithNullEnumerableThrows()
    {
        // Act & assert
        Assert.Throws<ArgumentNullException>(
            () => _ = new CompositeDataSourceAttribute(((IEnumerable<BaseDataSourceAttribute>)null)!));
    }

    [Test]
    public async Task AttributesIsCorrectWhenInitializedWithEnumerable()
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
        var result = sut.Attributes;
        // Assert
        await Assert.That(result).IsEquivalentTo(attributes);
    }

    [Test]
    public async Task GetDataWithNullGeneratorMetadataThrows()
    {
        // Arrange
        var sut = new CompositeDataSourceAttribute();

        // Act & assert
        await Assert.That(() => sut.GenerateDataSources(null!)
            .Select(x => x()).ToArray()).ThrowsException();
    }

    [Test]
    public async Task GetDataOnMethodWithNoParametersReturnsNoTheory()
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