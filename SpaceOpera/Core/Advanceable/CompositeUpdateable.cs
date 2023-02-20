namespace SpaceOpera.Core.Advanceable
{
    public class CompositeUpdateable : IUpdateable, IEnumerable<IUpdateable>
    {
        private readonly List<IUpdateable> _updateables;

        public CompositeUpdateable()
        {
            _updateables = new List<IUpdateable>();
        }

        public CompositeUpdateable(IEnumerable<IUpdateable> tickables)
        {
            _updateables = tickables.ToList();
        }

        public IEnumerator<IUpdateable> GetEnumerator()
        {
            return _updateables.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IUpdateable updateable)
        {
            _updateables.Add(updateable);
        }

        public void Update(long delta)
        {
            _updateables.ForEach(x => x.Update(delta));
        }
    }
}
