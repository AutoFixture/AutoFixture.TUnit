namespace TestTypeFoundation;

public class InternalGetterPropertyHolder<T>(T property)
{
    public T Property { internal get; set; } = property;
}