namespace AutoFixture.TUnit.Tests.TestTypes;

internal class DerivedArgumentsAutoDataAttribute(Func<IFixture> fixtureFactory, params object?[] values)
    : ArgumentsAutoDataAttribute(fixtureFactory, values);