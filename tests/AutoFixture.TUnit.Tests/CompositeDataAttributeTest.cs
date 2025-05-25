using System.Reflection;
using AutoFixture.TUnit.Tests.TestTypes;
using TUnit.Assertions.AssertConditions.Throws;

namespace AutoFixture.TUnit.Tests;

public class CompositeDataAttributeTest
{
    [Test]
    public async Task SutIsDataAttribute()
    {
        // Arrange & Act
        var sut = new CompositeDataAttribute();

        // Assert
        await Assert.That(sut).IsAssignableTo<BaseDataSourceAttribute>();
    }

    [Test]
    public async Task InitializeWithNullArrayThrows()
    {
        // Arrange
        // Act & assert
        await Assert.That(() => new CompositeDataAttribute(null!)).ThrowsExactly<ArgumentNullException>();
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

        var sut = new CompositeDataAttribute(attributes);
        // Act
        IEnumerable<BaseDataSourceAttribute> result = sut.Attributes;
        // Assert
        await Assert.That(result).IsEquivalentTo(attributes);
    }

    [Test]
    public void InitializeWithNullEnumerableThrows()
    {
        // Arrange
        // Act & assert
        Assert.Throws<ArgumentNullException>(
            () => new CompositeDataAttribute((IReadOnlyCollection<BaseDataSourceAttribute>)null!));
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

        var sut = new CompositeDataAttribute(attributes);
        // Act
        var result = sut.Attributes;
        // Assert
        await Assert.That(result).IsEquivalentTo(attributes);
    }

    [Test]
    public async Task GetDataWithNullMethodThrows()
    {
        // Arrange
        var sut = new CompositeDataAttribute();
        // Act & assert
        await Assert.That(() => sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(null!, null!))
            .Select(x => x()).ToArray()).ThrowsException();
    }

    [Test]
    public async Task GetDataOnMethodWithNoParametersReturnsNoTheory()
    {
        // Arrange
        Action a = () => { };
        var method = a.GetMethodInfo();

        var sut = new CompositeDataAttribute(
            new FakeDataAttribute(method, []),
            new FakeDataAttribute(method, []),
            new FakeDataAttribute(method, []));

        // Act
        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(method))
                .Select(x => x())
                .ToArray();

        // Assert
        await Assert.That(result).All().Satisfy(row => row.IsEmpty());
    }
}