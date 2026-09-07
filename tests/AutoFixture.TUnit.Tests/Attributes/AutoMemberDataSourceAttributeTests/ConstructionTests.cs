namespace AutoFixture.TUnit.Tests.Attributes.AutoMemberDataSourceAttributeTests;

public class ConstructionTests
{
    [Test]
    public async Task Constructor_WhenCreated_IsBaseDataSourceAttribute()
    {
        var sut = new AutoMemberDataSourceAttribute(Guid.NewGuid().ToString());

        await Assert.That(sut).IsAssignableTo<BaseDataSourceAttribute>();
    }

    [Test]
    public async Task Constructor_WhenMemberNameAndParameters_SetsMembers()
    {
        var memberName = Guid.NewGuid().ToString();
        object[] parameters = ["value-one", 3, 12.2f];

        var sut = new AutoMemberDataSourceAttribute(memberName, parameters);

        await Assert.That(sut.MemberName).IsEqualTo(memberName);
        await Assert.That(sut.Parameters).IsEqualTo(parameters);
        await Assert.That(sut.MemberType).IsNull();
        await Assert.That(sut.FixtureFactory).IsNotNull();
    }

    [Test]
    public async Task Constructor_WhenTypeMemberNameAndParameters_SetsMembers()
    {
        var memberName = Guid.NewGuid().ToString();
        object[] parameters = ["value-one", 3, 12.2f];
        var testType = typeof(ConstructionTests);

        var sut = new AutoMemberDataSourceAttribute(testType, memberName, parameters);

        await Assert.That(sut.MemberName).IsEqualTo(memberName);
        await Assert.That(sut.Parameters).IsEqualTo(parameters);
        await Assert.That(sut.MemberType).IsEqualTo(testType);
        await Assert.That(sut.FixtureFactory).IsNotNull();
    }

    [Test]
    public async Task Constructor_WhenMemberNameIsNull_ThrowsArgumentNullException()
    {
        await Assert.That(() => new AutoMemberDataSourceAttribute(null))
            .ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Constructor_WhenParametersIsNull_TreatsAsSingleNullArgument()
    {
        var memberName = Guid.NewGuid().ToString();

        var actual = new AutoMemberDataSourceAttribute(memberName, null);

        var value = await Assert.That(actual.Parameters).HasSingleItem();
        await Assert.That(value).IsNull();
    }

    [Test]
    public void Constructor_WhenMemberTypeIsNull_DoesNotThrow()
    {
        var memberName = Guid.NewGuid().ToString();

        _ = new AutoMemberDataSourceAttribute(null, memberName);
    }
}
