using System.Reflection;

namespace AutoFixture.TUnit.Internal;

internal sealed class TestParameter
{
    private readonly Lazy<ICustomization> lazyCustomization;
    private readonly Lazy<FrozenAttribute?> lazyFrozenAttribute;

    public TestParameter(ParameterInfo parameterInfo)
    {
        this.lazyCustomization = new Lazy<ICustomization>(
            () => GetCustomization(parameterInfo));
        this.lazyFrozenAttribute = new Lazy<FrozenAttribute?>(
            () => parameterInfo.GetCustomAttributes()
                .OfType<FrozenAttribute>().FirstOrDefault());
        this.ParameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));
    }

    public ParameterInfo ParameterInfo { get; }

    public ICustomization GetCustomization() => this.lazyCustomization.Value;

    public ICustomization GetCustomization(object? value)
    {
        var frozenAttribute = this.lazyFrozenAttribute.Value;

        if (frozenAttribute is null)
        {
            return NullCustomization.Instance;
        }

        return new FrozenValueCustomization(
            new ParameterFilter(this.ParameterInfo, frozenAttribute.By),
            value);
    }

    private static ICustomization GetCustomization(ParameterInfo parameter)
    {
        var customizations = parameter.GetCustomAttributes()
            .OfType<IParameterCustomizationSource>()
            .OrderBy(x => x, new CustomizeAttributeComparer())
            .Select(x => x.GetCustomization(parameter))
            .ToArray();

        return customizations switch
        {
            { Length: 0 } => NullCustomization.Instance,
            { Length: 1 } => customizations[0],
            _ => new CompositeCustomization(customizations),
        };
    }

    public static TestParameter From(ParameterInfo parameterInfo) => new(parameterInfo);
}