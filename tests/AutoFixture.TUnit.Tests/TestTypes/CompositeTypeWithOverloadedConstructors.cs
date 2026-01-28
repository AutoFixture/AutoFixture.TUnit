namespace AutoFixture.TUnit.Tests.TestTypes;

public class CompositeTypeWithOverloadedConstructors<T>
{
    public CompositeTypeWithOverloadedConstructors(params T[] items)
        : this(items.AsEnumerable())
    {
    }

    public CompositeTypeWithOverloadedConstructors(IList<T> items)
        : this(items.AsEnumerable())
    {
    }

    public CompositeTypeWithOverloadedConstructors(IEnumerable<T> items)
    {
        this.Items = items;
    }

    public IEnumerable<T> Items { get; }
}