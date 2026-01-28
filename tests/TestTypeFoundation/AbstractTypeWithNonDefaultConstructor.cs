namespace TestTypeFoundation;

public abstract class AbstractTypeWithNonDefaultConstructor<T>
{
    protected AbstractTypeWithNonDefaultConstructor(T value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));

        this.Property = value;
    }

    public T Property { get; }
}