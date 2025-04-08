using AutoFixture.Kernel;

namespace AutoFixture.TUnit.Internal
{
    internal class FrozenValueCustomization(IRequestSpecification specification, object? value) : ICustomization
    {
        private readonly IRequestSpecification specification = specification ?? throw new ArgumentNullException(nameof(specification));

        public void Customize(IFixture fixture)
        {
            var builder = new FilteringSpecimenBuilder(
                builder: new FixedBuilder(value),
                specification: this.specification);

            fixture.Customizations.Insert(0, builder);
        }
    }
}