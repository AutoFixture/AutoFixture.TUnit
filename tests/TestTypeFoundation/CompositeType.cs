namespace TestTypeFoundation;

public class CompositeType : AbstractType
{
    public CompositeType(IEnumerable<AbstractType> types)
        : this(types.ToArray())
    {
    }

    public CompositeType(params AbstractType[] types)
    {
        this.Types = types ?? throw new ArgumentNullException(nameof(types));
    }

    public IEnumerable<AbstractType> Types { get; }
}