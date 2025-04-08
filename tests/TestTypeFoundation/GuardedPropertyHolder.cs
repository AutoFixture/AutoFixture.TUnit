namespace TestTypeFoundation
{
    public class GuardedPropertyHolder<T>
        where T : class
    {
        private T _property;

        public T Property
        {
            get
            {
                return this._property;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this._property = value;
            }
        }
    }
}