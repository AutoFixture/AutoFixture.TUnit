using AutoFixture.TUnit.Tests.Internal.DataGeneratorMetadataSupport;
using AutoFixture.TUnit.Tests.TestTypes;
using AutoFixture.TUnit.Tests.TestTypes.DataSourceFixtures;

namespace AutoFixture.TUnit.Tests.Attributes.AutoMemberDataSourceAttributeTests;

internal static class AutoMemberRowCollect
{
    public static object[][] FromMember(string memberName, string testMethodName)
    {
        var sut = new AutoMemberDataSourceAttribute(typeof(MemberDataShapes), memberName);
        var testMethod = typeof(SampleTestType).GetMethod(testMethodName)!;
        return sut.GenerateDataSources(DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod))
            .Select(x => x())
            .ToArray();
    }
}
