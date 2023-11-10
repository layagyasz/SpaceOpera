namespace SpaceOpera.View.Components.Dynamics
{
    public class FunctionRange<T> : IRange<T>
    {
        private readonly Func<IEnumerable<T>> _rangeFn;

        public FunctionRange(Func<IEnumerable<T>> rangeFn)
        {
            _rangeFn = rangeFn;
        }

        public IEnumerable<T> GetRange()
        {
            return _rangeFn();
        }
    }
}
