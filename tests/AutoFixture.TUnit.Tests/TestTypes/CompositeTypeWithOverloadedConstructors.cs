namespace AutoFixture.TUnit.Tests.TestTypes;

public class CompositeTypeWithOverloadedConstructors<T>(IEnumerable<T> items)
{
    public CompositeTypeWithOverloadedConstructors(params T[] items)
        : this(items.AsEnumerable())
    {
    }

    public CompositeTypeWithOverloadedConstructors(IList<T> items)
        : this(items.AsEnumerable())
    {
    }

    public IEnumerable<T> Items { get; } = items;
}