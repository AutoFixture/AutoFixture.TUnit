namespace TestTypeFoundation;

public class GuardedConstructorHost<T>
    where T : class
{
    public GuardedConstructorHost(T item)
    {
        this.Item = item ?? throw new ArgumentNullException(nameof(item));
    }

    public T Item { get; }
}