using AutoFixture.TUnit.Internal;
using AutoFixture.TUnit.Tests.Internal.DataGeneratorMetadataSupport;
using AutoFixture.TUnit.Tests.TestTypes;
using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests.Internal.DataSources.MethodDataSourceTests;

/// <summary>
/// Internal MethodDataSource construction and wiring leftovers.
/// Row shapes are covered via AutoMemberDataSourceAttribute.
/// </summary>
public class ConstructionTests
{
    public static IEnumerable<object[]> ObjectArrayRows()
    {
        yield return ["hello", 1, new RecordType<string>("world")];
        yield return ["foo", 2, new RecordType<string>("bar")];
    }

    [Test]
    public async Task Constructor_WhenCreated_IsDataSource()
    {
        var methodInfo = typeof(ConstructionTests).GetMethod(nameof(this.Constructor_WhenCreated_IsDataSource));
        var sut = new MethodDataSource(methodInfo);

        await Assert.That(sut).IsAssignableTo<DataSource>();
    }

    [Test]
    public async Task Constructor_WhenMethodInfoIsNull_ThrowsArgumentNullException()
    {
        await Assert.That(() => new MethodDataSource(null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Constructor_WhenArgumentsIsNull_ThrowsArgumentNullException()
    {
        var methodInfo = typeof(ConstructionTests).GetMethod(nameof(this.Constructor_WhenCreated_IsDataSource));

        await Assert.That(() => new MethodDataSource(methodInfo, null)).ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Constructor_WhenArgumentsProvided_SetsMembers()
    {
        var methodInfo = typeof(ConstructionTests).GetMethod(nameof(this.Constructor_WhenCreated_IsDataSource));
        object[] arguments = [new object()];

        var sut = new MethodDataSource(methodInfo, arguments);

        await Assert.That(sut.MethodInfo).IsEqualTo(methodInfo);
        await Assert.That(sut.Arguments).IsEquivalentTo(arguments);
    }

    [Test]
    public async Task GetData_WhenInvoked_CallsSourceMethod()
    {
        object[][] expected = [
            [ "hello", 1, new RecordType<string>("world") ],
            ["foo", 2, new RecordType<string>("bar")]
        ];
        var sourceMethod = typeof(ConstructionTests).GetMethod(nameof(ObjectArrayRows));
        var testMethod = typeof(SampleTestType).GetMethod(nameof(SampleTestType.TestMethodWithReferenceTypeParameter));
        var sut = new MethodDataSource(sourceMethod);

        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            .Select(x => x());

        await Assert.That(result).IsEquivalentTo(expected);
    }
}
