using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe
{
    class Galaxy
    {
        public double Radius { get; }
        public List<StarSystem> Systems { get; }

        public Galaxy(double Radius, IEnumerable<StarSystem> Systems)
        {
            this.Radius = Radius;
            this.Systems = Systems.ToList();
        }

        public IEnumerable<Tuple<StarSystem, StarSystem>> GetTransits()
        {
            var closed = new HashSet<StarSystem>();
            foreach (var system in Systems)
            {
                closed.Add(system);
                foreach (var transit in system.Transits.Values.Where(x => !closed.Contains(x.TransitSystem)))
                {
                    yield return new Tuple<StarSystem, StarSystem>(system, transit.TransitSystem);
                }
            }
        }
    }
}