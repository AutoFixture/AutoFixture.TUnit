namespace TestTypeFoundation;

public class IndexedPropertyHolder<T>
{
    private readonly List<T> items = new();

    public T this[int index]
    {
        get { return this.items[index]; }
        set { this.items[index] = value; }
    }
}