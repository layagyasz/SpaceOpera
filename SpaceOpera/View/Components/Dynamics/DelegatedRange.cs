namespace SpaceOpera.View.Components.Dynamics
{
    public class DelegatedRange<T>
    {
        private KeyRange<T>? _delegate;

        public IEnumerable<T> GetRange()
        {
            return _delegate?.Invoke() ?? Enumerable.Empty<T>();
        }

        public void SetRange(KeyRange<T>? @delegate)
        {
            _delegate = @delegate;
        }
    }
}
