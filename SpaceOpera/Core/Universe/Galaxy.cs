namespace SpaceOpera.Core.Universe
{
    public class Galaxy
    {
        public float Radius { get; }
        public List<StarSystem> Systems { get; }

        public Galaxy(float radius, IEnumerable<StarSystem> systems)
        {
            Radius = radius;
            Systems = systems.ToList();
        }

        public int GetSize()
        {
            return Systems.Sum(x => x.GetSize());
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