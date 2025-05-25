namespace AutoFixture.TUnit.Tests.TestTypes;

public class DerivedAutoMemberDataSourceAttribute : AutoMemberDataSourceAttribute
{
    public DerivedAutoMemberDataSourceAttribute(Func<IFixture> fixtureFactory, string memberName, params object[] parameters)
        : base(fixtureFactory, memberName, parameters)
    {
    }

    public DerivedAutoMemberDataSourceAttribute(Func<IFixture> fixtureFactory, Type memberType, string memberName, params object[] parameters)
        : base(fixtureFactory, memberType, memberName, parameters)
    {
    }
}