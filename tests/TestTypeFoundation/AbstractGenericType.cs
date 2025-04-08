namespace TestTypeFoundation
{
    public abstract class AbstractGenericType<T>(T t)
    {
        public T Value { get; } = t;
    }
}