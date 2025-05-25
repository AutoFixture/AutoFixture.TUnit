namespace TestTypeFoundation;

public static class GuardedStaticMethodOnStaticTypeHost
{
    public static void Method(object argument)
    {
        if (argument is null) throw new ArgumentNullException(nameof(argument));
    }
}