namespace TestTypeFoundation
{
    public class TypeWithIndexer
    {
        private readonly Dictionary<string, string> _dict = new();

        public string this[string index]
        {
            get
            {
                return this._dict[index];
            }
            set
            {
                this._dict[index] = value;
            }
        }
    }
}
