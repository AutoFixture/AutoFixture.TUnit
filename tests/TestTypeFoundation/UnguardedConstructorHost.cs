using System.Reflection;

namespace TestTypeFoundation;

public class UnguardedConstructorHost<T>(T item)
{
    public T Item { get; } = item;

    private static ConstructorInfo GetConstructor()
    {
        var typeInfo = typeof(UnguardedConstructorHost<T>)
            .GetTypeInfo();

        return typeInfo.DeclaredConstructors.Single();
    }

    public static ParameterInfo GetParameter()
    {
        return GetConstructor().GetParameters().Single();
    }
}