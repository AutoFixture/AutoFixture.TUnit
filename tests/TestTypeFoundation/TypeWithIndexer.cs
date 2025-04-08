namespace TestTypeFoundation;

public class TypeWithIndexer
{
    private readonly Dictionary<string, string> _dict = new();

    public string this[string index]
    {
        get
        {
            return _dict[index];
        }
        set
        {
            _dict[index] = value;
        }
    }
}