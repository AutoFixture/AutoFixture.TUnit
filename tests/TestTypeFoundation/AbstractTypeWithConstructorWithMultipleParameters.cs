namespace TestTypeFoundation;

public abstract class AbstractTypeWithConstructorWithMultipleParameters<T1, T2>(
    T1 parameter1,
    T2 parameter2)
{
    public T1 Property1 { get; } = parameter1;

    public T2 Property2 { get; } = parameter2;
}