using AutoFixture.TUnit.Tests.TestTypes.DataSourceFixtures;

namespace AutoFixture.TUnit.Tests.Attributes.AutoMemberDataSourceAttributeTests;

/// <summary>
/// Empty member sequences cannot be asserted via a [Test] body (no cases run),
/// so they stay as direct GetData checks.
/// </summary>
public class EmptySequenceRowTests
{
    [Test]
    public async Task GetData_WhenMemberReturnsEmptyObjectArraySequence_YieldsNoRows()
    {
        var result = AutoMemberRowCollect.FromMember(
            nameof(MemberDataShapes.EmptyObjectArrays),
            nameof(TestTypes.SampleTestType.TestMethodWithTwoParameters));

        await Assert.That(result).IsEmpty();
    }

    [Test]
    public async Task GetData_WhenMemberReturnsEmptyAsyncSequence_YieldsNoRows()
    {
        var result = AutoMemberRowCollect.FromMember(
            nameof(MemberDataShapes.EmptyAsyncObjectArrays),
            nameof(TestTypes.SampleTestType.TestMethodWithTwoParameters));

        await Assert.That(result).IsEmpty();
    }
}
