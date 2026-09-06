using AutoFixture.TUnit.Tests.Internal.DataGeneratorMetadataSupport;
using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests.Attributes.AutoClassDataSourceAttributeTests;

internal static class AutoClassRowCollect
{
    public static object[][] FromType(Type sourceType, string testMethodName)
    {
        var sut = new AutoClassDataSourceAttribute(sourceType);
        var testMethod = typeof(SampleTestType).GetMethod(testMethodName)!;
        return sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            .Select(x => x())
            .ToArray();
    }
}
