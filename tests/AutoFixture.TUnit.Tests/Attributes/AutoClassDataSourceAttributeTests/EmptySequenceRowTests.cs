using AutoFixture.TUnit.Tests.TestTypes;
using AutoFixture.TUnit.Tests.TestTypes.DataSourceFixtures;

namespace AutoFixture.TUnit.Tests.Attributes.AutoClassDataSourceAttributeTests;

/// <summary>
/// Empty class sequences cannot be asserted via a [Test] body (no cases run),
/// so they stay as direct GetData checks.
/// </summary>
public class EmptySequenceRowTests
{
    [Test]
    public async Task GetData_WhenClassYieldsEmptyAsyncSequence_YieldsNoRows()
    {
        var result = AutoClassRowCollect.FromType(
            typeof(EmptyAsyncObjectArrayClassData),
            nameof(SampleTestType.TestMethodWithTwoParameters));

        await Assert.That(result).IsEmpty();
    }
}
