namespace AutoFixture.TUnit.Tests.TestTypes;

internal class DerivedAutoArgumentsAttribute
    : AutoArgumentsAttribute
{
    public DerivedAutoArgumentsAttribute(Func<IFixture> fixtureFactory, params object[] values)
        : base(fixtureFactory, values)
    {
    }
}