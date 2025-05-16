namespace AutoFixture.TUnit.Tests.TestTypes;

public class DerivedAutoDataAttribute : AutoDataAttribute
{
    public DerivedAutoDataAttribute(Func<IFixture> fixtureFactory)
        : base(fixtureFactory)
    {
    }
}