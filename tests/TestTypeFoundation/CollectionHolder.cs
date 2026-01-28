namespace TestTypeFoundation;

public class CollectionHolder<T>
{
    public ICollection<T> Collection { get; private set; } = new List<T>();
}