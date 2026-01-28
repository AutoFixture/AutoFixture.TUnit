namespace AutoFixture.TUnit.Tests.TestTypes;

public class MyClass
{
    public T Echo<T>(T item) => item;
}