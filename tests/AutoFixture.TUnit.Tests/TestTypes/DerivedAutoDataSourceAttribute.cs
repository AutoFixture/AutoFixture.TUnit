namespace AutoFixture.TUnit.Tests.TestTypes;

public class DerivedAutoDataSourceAttribute : AutoDataSourceAttribute
{
    public DerivedAutoDataSourceAttribute(Func<IFixture> fixtureFactory)
        : base(fixtureFactory)
    {
    }
}