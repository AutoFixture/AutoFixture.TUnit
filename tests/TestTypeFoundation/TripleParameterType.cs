namespace TestTypeFoundation;

public class TripleParameterType<T1, T2, T3>(T1 parameter1, T2 parameter2, T3 parameter3)
{
    public T1 Parameter1 { get; private set; } = parameter1;

    public T2 Parameter2 { get; private set; } = parameter2;

    public T3 Parameter3 { get; private set; } = parameter3;
}