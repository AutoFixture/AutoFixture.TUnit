namespace AutoFixture.TUnit.Internal;

internal sealed class Argument
{
    public Argument(TestParameter parameter, object? value)
    {
        this.Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        this.Value = value;
    }

    public TestParameter Parameter { get; }

    public object? Value { get; }

    public ICustomization GetCustomization()
        => this.Parameter.GetCustomization(this.Value);
}