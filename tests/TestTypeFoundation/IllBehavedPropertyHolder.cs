namespace TestTypeFoundation
{
    public class IllBehavedPropertyHolder<T>
    {
        private T _propertyIllBehavedSet;

        public T PropertyIllBehavedGet
        {
            get
            {
                return default(T);
            }

            set
            {
            }
        }

        public T PropertyIllBehavedSet
        {
            get
            {
                return this._propertyIllBehavedSet;
            }

            set
            {
                this._propertyIllBehavedSet = default(T);
            }
        }
    }
}