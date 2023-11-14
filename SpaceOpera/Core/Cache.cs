namespace SpaceOpera.Core
{
    public class Cache<T>
    {
        public delegate T CacheLoader();

        private readonly CacheLoader _loader;

        private T? _value;
        private bool _valid;

        public Cache(CacheLoader loader)
        {
            _loader = loader;
        }

        public T Get()
        {
            lock(this)
            {
                if (!_valid)
                {
                    _valid = true;
                    _value = _loader();
                }
                return _value!;
            }
        }

        public void Invalidate()
        {
            lock (this)
            {
                _valid = false;
            }
        }
    }
}
