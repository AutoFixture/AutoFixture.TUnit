namespace TestTypeFoundation;

public class GenericType<T>
    where T : class
{
    public GenericType(T value)
    {
        this.Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    private T Value { get; }
}