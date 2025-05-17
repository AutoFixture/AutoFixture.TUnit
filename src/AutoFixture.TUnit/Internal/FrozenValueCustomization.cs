using AutoFixture.Kernel;

namespace AutoFixture.TUnit.Internal;

internal sealed class FrozenValueCustomization : ICustomization
{
    private readonly IRequestSpecification specification;
    private readonly object? value;

    public FrozenValueCustomization(IRequestSpecification specification, object? value)
    {
        this.value = value;
        this.specification = specification ?? throw new ArgumentNullException(nameof(specification));
    }

    public void Customize(IFixture fixture)
    {
        var builder = new FilteringSpecimenBuilder(
            builder: new FixedBuilder(this.value),
            specification: this.specification);

        fixture.Customizations.Insert(0, builder);
    }
}