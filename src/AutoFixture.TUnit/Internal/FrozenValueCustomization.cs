using AutoFixture.Kernel;

namespace AutoFixture.TUnit.Internal;

internal sealed class FrozenValueCustomization(IRequestSpecification specification, object? value) : ICustomization
{
    private readonly IRequestSpecification _specification = specification ?? throw new ArgumentNullException(nameof(specification));

    public void Customize(IFixture fixture)
    {
        var builder = new FilteringSpecimenBuilder(
            builder: new FixedBuilder(value),
            specification: _specification);

        fixture.Customizations.Insert(0, builder);
    }
}