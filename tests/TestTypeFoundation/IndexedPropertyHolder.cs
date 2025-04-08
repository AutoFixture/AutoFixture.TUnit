namespace TestTypeFoundation;

public class IndexedPropertyHolder<T>
{
    private readonly List<T> _items =
    [
    ];

    public T this[int index]
    {
        get { return _items[index]; }
        set { _items[index] = value; }
    }
}