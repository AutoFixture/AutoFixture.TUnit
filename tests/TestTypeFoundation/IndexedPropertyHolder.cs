namespace TestTypeFoundation
{
    public class IndexedPropertyHolder<T>
    {
        private readonly List<T> _items = new();

        public T this[int index]
        {
            get { return this._items[index]; }
            set { this._items[index] = value; }
        }
    }
}
