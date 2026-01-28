namespace TestTypeFoundation;

public class TypeWithIndexer
{
    private readonly Dictionary<string, string> dict = new();

    public string this[string index]
    {
        get
        {
            return this.dict[index];
        }
        set
        {
            this.dict[index] = value;
        }
    }
}