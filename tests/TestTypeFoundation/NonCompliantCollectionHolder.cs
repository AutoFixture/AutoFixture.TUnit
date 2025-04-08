namespace TestTypeFoundation;

public class NonCompliantCollectionHolder<T>
{
    public ICollection<T> Collection { get; set; } = new List<T>();
}