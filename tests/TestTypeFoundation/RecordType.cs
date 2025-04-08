namespace TestTypeFoundation;

public class RecordType<T>(T value) : IEquatable<RecordType<T>>
{
    public T Value { get; } = value;

    public bool Equals(RecordType<T>? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as RecordType<T>);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Value);
    }
}