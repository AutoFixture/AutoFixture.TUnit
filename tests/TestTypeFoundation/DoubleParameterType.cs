namespace TestTypeFoundation
{
    public class DoubleParameterType<T1, T2>(T1 parameter1, T2 parameter2)
    {
        public T1 Parameter1 { get; private set; } = parameter1;

        public T2 Parameter2 { get; private set; } = parameter2;
    }
}
