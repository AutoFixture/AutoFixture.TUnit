using AutoFixture.Kernel;

namespace AutoFixture.TUnit.Tests.TestTypes;

internal class FixedParameterBuilder<T>(string name, T value)
    : FilteringSpecimenBuilder(new FixedBuilder(value), new ParameterSpecification(typeof(T), name));