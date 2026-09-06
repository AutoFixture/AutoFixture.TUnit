using AutoFixture.TUnit.Tests.Internal.DataGeneratorMetadataSupport;
using AutoFixture.TUnit.Tests.TestTypes;

namespace AutoFixture.TUnit.Tests.Attributes.AutoClassDataSourceAttributeTests;

public class NullRowTests
{
    [Test]
    public async Task GetData_WhenClassYieldsNullRows_ThrowsInvalidOperationException()
    {
        var sut = new AutoClassDataSourceAttribute(typeof(ClassWithNullTestData));
        var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
        var metadata = DataGeneratorMetadataHelper.CreateDataGeneratorMetadata(testMethod);

        var act = () => sut.GenerateDataSources(metadata)
            .Select(row => row())
            .ToArray();

        await Assert.That(act).ThrowsExactly<InvalidOperationException>();
    }
}
