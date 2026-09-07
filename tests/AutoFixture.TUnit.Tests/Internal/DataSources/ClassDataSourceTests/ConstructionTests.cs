using AutoFixture.TUnit.Internal;
using AutoFixture.TUnit.Tests.Internal.DataGeneratorMetadataSupport;
using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests.Internal.DataSources.ClassDataSourceTests;

/// <summary>
/// Internal ClassDataSource construction leftovers. Row shapes via AutoClassDataSourceAttribute.
/// </summary>
public class ConstructionTests
{
    [Test]
    public async Task Constructor_WhenCreated_IsDataSource()
    {
        var sut = new ClassDataSource(typeof(object));

        await Assert.That(sut).IsAssignableTo<IDataSource>();
    }

    [Test]
    public async Task Constructor_WhenTypeIsNull_ThrowsArgumentNullException()
    {
        await Assert.That(() => _ = new ClassDataSource(null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Constructor_WhenParametersIsNull_ThrowsArgumentNullException()
    {
        await Assert.That(() => _ = new ClassDataSource(typeof(object), null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Type_WhenRead_ReturnsConstructorArgument()
    {
        var expected = typeof(object);
        var sut = new ClassDataSource(expected);

        await Assert.That(sut.Type).IsEqualTo(expected);
    }

    [Test]
    public async Task Parameters_WhenRead_ReturnsConstructorArguments()
    {
        object[] expected = [new object()];
        var sut = new ClassDataSource(typeof(object), expected);

        await Assert.That(sut.Parameters).IsEquivalentTo(expected);
    }

    [Test]
    public async Task GetData_WhenConstructorParametersDontMatch_Throws()
    {
        object[] parameters = ["a", 1];
        var sut = new ClassDataSource(typeof(object), parameters);
        var method = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithReferenceTypeParameter));

        await Assert.That(() => sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(method)).ToArray())
            .ThrowsException();
    }

    [Test]
    public async Task GetData_WhenConstructorParametersMatch_AppliesThem()
    {
        object[] parameters = [new object[] { "y", 25 }];
        var sut = new ClassDataSource(typeof(DelegatingTestData), parameters);
        var method = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithReferenceTypeParameter));

        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(method)).ToArray();

        await Assert.That(result.Single()).IsEquivalentTo<object[], object>(["y", 25]);
    }
}
