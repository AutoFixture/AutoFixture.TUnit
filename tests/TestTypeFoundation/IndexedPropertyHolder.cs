namespace TestTypeFoundation
{
    public class IndexedPropertyHolder<T>
    {
        private readonly List<T> _items = new();

        public T this[int index]
        {
            get { return _items[index]; }
            set { _items[index] = value; }
        }
    }
}
