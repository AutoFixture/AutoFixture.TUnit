using TestTypeFoundation;

namespace AutoFixture.TUnit.Tests.TestTypes;

public class ExampleTestClass
{
    public void TestMethod(int a, string b, EnumType c, Tuple<string, int> d)
    {
    }
}

public class ExampleTestClass<T1, T2, T3, T4>
{
    public void TestMethod(T1 a, T2 b, T3 c, T4 d)
    {
    }
}
