namespace TestTypeFoundation;

public class GuardedPropertyHolder<T>
    where T : class
{
    private T property;

    public T Property
    {
        get => this.property;
        set => this.property = value ?? throw new ArgumentNullException(nameof(value));
    }
}