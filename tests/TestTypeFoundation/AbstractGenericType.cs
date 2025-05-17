namespace TestTypeFoundation;

public abstract class AbstractGenericType<T>
{
    protected AbstractGenericType(T value)
    {
        this.Value = value;
    }

    public T Value { get; }
}