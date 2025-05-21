namespace AutoFixture.TUnit.Tests.TestTypes;

public class DerivedAutoClassDataSourceAttribute : AutoClassDataSourceAttribute
{
    public DerivedAutoClassDataSourceAttribute(Type sourceType)
        : base(sourceType)
    {
    }

    public DerivedAutoClassDataSourceAttribute(Func<IFixture> fixtureFactory, Type sourceType, params object[] parameters)
        : base(fixtureFactory, sourceType, parameters)
    {
    }
}