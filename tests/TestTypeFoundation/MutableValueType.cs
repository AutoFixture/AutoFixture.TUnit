namespace TestTypeFoundation;

public struct MutableValueType(object property1, object property2, object property3)
{
    public object Property1 { get; set; } = property1;

    public object Property2 { get; set; } = property2;

    public object Property3 { get; set; } = property3;
}