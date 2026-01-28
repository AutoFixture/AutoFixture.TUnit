namespace TestTypeFoundation;

public class ItemHolder<T1, T2>
{
    public ItemHolder()
    {
    }

    public ItemHolder(T1 item)
        : this([item], [
        ])
    {
    }

    public ItemHolder(T2 item)
        : this([
        ], [item])
    {
    }

    private ItemHolder(T1[] t1S, T2[] t2S)
    {
        this.Item1S = t1S;
        this.Item2S = t2S;
    }

    public IEnumerable<T1> Item1S { get; }

    public IEnumerable<T2> Item2S { get; }
}

/* Note that constructors must be unordered because this class is used to test that
 * constructors are correctly ordered by various implementations of IMethodQuery. For that
 * reason, please don't be a boy scout and order constructors 'nicely'. */
public class ItemHolder<T>
{
    public ItemHolder(T x, T y, T z)
        : this([x, y, z])
    {
    }

    public ItemHolder(T item)
        : this([item])
    {
    }

    public ItemHolder()
    {
    }

    public ItemHolder(T x, T y)
        : this([x, y])
    {
    }

    private ItemHolder(T[] items)
    {
        this.Items = items;
    }

    public IEnumerable<T> Items { get; }
}