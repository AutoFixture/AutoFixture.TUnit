namespace AutoFixture.TUnit.Internal;

internal sealed class Argument(TestParameter parameter, object? value)
{
    public TestParameter Parameter { get; } = parameter ?? throw new ArgumentNullException(nameof(parameter));

    public object? Value { get; } = value;

    public ICustomization GetCustomization() => Parameter.GetCustomization(Value);
}