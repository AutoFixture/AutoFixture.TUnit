using AutoFixture.TUnit.Internal;
using AutoFixture.TUnit.Tests.Internal.DataGeneratorMetadataSupport;
using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests.Internal.DataSources.MethodDataSourceTests;

/// <summary>
/// Leftover Internal coverage for branches awkward to express via public attributes
/// (nested Task unwrap). Prefer AutoMemberDataSourceAttribute shape tests for normal cases.
/// </summary>
public class ResultShapeLeftoverTests
{
    [Test]
    public async Task GetData_WhenNestedTaskOfTupleSequence_UnwrapsAllTasksAndExpandsTuples()
    {
        var sourceMethod = typeof(ResultShapeLeftoverTests).GetMethod(nameof(NestedTaskOfTupleSequence));
        var testMethod = typeof(SampleTestType).GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));
        var sut = new MethodDataSource(sourceMethod!);

        var result = sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod!))
            .Select(x => x())
            .ToArray();

        await Assert.That(result).IsEquivalentTo<object[][], object[]>([["a", 1], ["b", 2]]);
    }

    public static Task<Task<IEnumerable<(string, int)>>> NestedTaskOfTupleSequence() =>
        Task.FromResult(Task.FromResult<IEnumerable<(string, int)>>([("a", 1), ("b", 2)]));
}
