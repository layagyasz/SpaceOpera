using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Advanceable
{
    class CompositeTickable : ITickable, IEnumerable<ITickable>
    {
        private readonly List<ITickable> _Tickables;

        public CompositeTickable()
        {
            _Tickables = new List<ITickable>();
        }

        public CompositeTickable(IEnumerable<ITickable> Tickables)
        {
            _Tickables = Tickables.ToList();
        }

        public IEnumerator<ITickable> GetEnumerator()
        {
            return _Tickables.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ITickable Tickable)
        {
            _Tickables.Add(Tickable);
        }

        public void Tick()
        {
            _Tickables.ForEach(x => x.Tick());
        }
    }
}