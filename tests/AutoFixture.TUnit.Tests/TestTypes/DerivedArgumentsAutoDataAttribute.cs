namespace AutoFixture.TUnit.Tests.TestTypes;

internal class DerivedArgumentsAutoDataAttribute
    : AutoArgumentsAttribute
{
    public DerivedArgumentsAutoDataAttribute(Func<IFixture> fixtureFactory, params object[] values)
        : base(fixtureFactory, values)
    {
    }
}