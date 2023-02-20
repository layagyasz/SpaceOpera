namespace SpaceOpera.Core.Advanceable
{
    public class CompositeTickable : ITickable, IEnumerable<ITickable>
    {
        private readonly List<ITickable> _tickables;

        public CompositeTickable()
        {
            _tickables = new List<ITickable>();
        }

        public CompositeTickable(IEnumerable<ITickable> tickables)
        {
            _tickables = tickables.ToList();
        }

        public IEnumerator<ITickable> GetEnumerator()
        {
            return _tickables.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ITickable tickable)
        {
            _tickables.Add(tickable);
        }

        public void Tick()
        {
            _tickables.ForEach(x => x.Tick());
        }
    }
}