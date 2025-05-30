using AutoFixture.Kernel;

namespace AutoFixture.TUnit.Tests.TestTypes;

internal class FixedParameterBuilder<T>
    : FilteringSpecimenBuilder
{
    public FixedParameterBuilder(string name, T value)
        : base(new FixedBuilder(value), new ParameterSpecification(typeof(T), name))
    {
    }
}