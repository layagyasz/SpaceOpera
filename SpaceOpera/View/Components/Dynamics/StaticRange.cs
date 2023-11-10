namespace SpaceOpera.View.Components.Dynamics
{
    public class StaticRange<T> : IRange<T>
    {
        private readonly List<T> _values = new();

        public StaticRange() { }

        public StaticRange(IEnumerable<T> values) 
        {
            _values.AddRange(values);
        }

        public void Add(T value)
        {
            _values.Add(value);
        }

        public void Clear() 
        {
            _values.Clear();
        }

        public IEnumerable<T> GetRange()
        {
            return _values;
        }

        public bool Remove(T value)
        {
            return _values.Remove(value);
        }

        public void Set(IEnumerable<T> values)
        {
            _values.Clear();
            _values.AddRange(values);
        }
    }
}
