namespace TestTypeFoundation;

public class TypeWithRefMethod<T>
{
    public void InvokeIt(T x, ref T y)
    {
        if (x is null) throw new ArgumentNullException(nameof(x));
        if (y is null) throw new ArgumentNullException(nameof(y));
    }
}