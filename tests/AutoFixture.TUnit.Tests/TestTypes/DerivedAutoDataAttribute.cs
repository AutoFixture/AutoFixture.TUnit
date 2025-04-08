namespace AutoFixture.TUnit.Tests.TestTypes;

public class DerivedAutoDataAttribute(Func<IFixture> fixtureFactory) : AutoDataAttribute(fixtureFactory);