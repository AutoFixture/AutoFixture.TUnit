namespace TestTypeFoundation;

public class EqualityResponder(bool equals)
{
    public override bool Equals(object obj)
    {
        return equals;
    }

    public override int GetHashCode()
    {
        return equals.GetHashCode();
    }
}