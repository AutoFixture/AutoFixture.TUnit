namespace TestTypeFoundation
{
    public class SingleParameterType<T>(T parameter)
    {
        public T Parameter { get; private set; } = parameter;
    }
}
