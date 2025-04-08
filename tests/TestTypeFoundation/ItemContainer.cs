namespace TestTypeFoundation
{
    public class ItemContainer<T>(params T[] items)
    {
        public ItemContainer(IEnumerable<T> items)
            : this(items.ToArray())
        {
        }

        public ItemContainer(IList<T> items)
            : this(items.ToArray())
        {
        }

        public IEnumerable<T> Items { get; } = items;
    }
}
