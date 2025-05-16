namespace TestTypeFoundation;

public class ItemContainer<T>
{
    public ItemContainer(IEnumerable<T> items)
        : this(items.ToArray())
    {
    }

    public ItemContainer(IList<T> items)
        : this(items.ToArray())
    {
    }

    public ItemContainer(params T[] items)
    {
        this.Items = items;
    }

    public IEnumerable<T> Items { get; }
}