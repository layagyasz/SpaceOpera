using OpenTK.Mathematics;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Voronoi;

namespace SpaceOpera.Core.Universe
{
    public class StarSystem
    {
        public string Name { get; private set; } = string.Empty;
        public Vector2 Position { get; }
        public Star Star { get; }
        public float InnerBoundary { get; }
        public float OuterBoundary { get; }
        public float TransitLimit { get; }
        public List<StellarBody> Orbiters { get; }
        public List<SolarOrbitRegion> OrbitalRegions { get; }
        public List<StarSystem>? Neighbors { get; private set; }
        public SortedList<double, TransitRegion> Transits { get; private set; }

        public StarSystem(
            Vector2 position,
            Star star, 
            float innerBoundary,
            float outerBoundary, 
            float transitLimit,
            IEnumerable<StellarBody> orbiters)
        {
            Position = position;
            Star = star;
            InnerBoundary = innerBoundary;
            OuterBoundary = outerBoundary;
            TransitLimit = transitLimit;
            Orbiters = orbiters.ToList();
            OrbitalRegions = orbiters.Select(x => new SolarOrbitRegion(new LocalOrbitRegion(x))).ToList();
            Transits = new SortedList<double, TransitRegion>();
        }

        public void AddTransit(StarSystem neighbor)
        {
            if (!Neighbors!.Contains(neighbor))
            {
                throw new ArgumentException(string.Format("{0} is not a neighbor of {1}", neighbor, this));
            }
            Transits.Add(MathUtils.Theta(Position, neighbor.Position), new TransitRegion(neighbor));
        }

        public bool ContainsFaction(Faction faction)
        {
            return Orbiters.Any(x => x.ContainsFaction(faction));
        }

        public void SetName(string name)
        {
            this.Name = name;
        }

        public void SetNeighbors(IEnumerable<StarSystem> neighbors)
        {
            Neighbors = neighbors.ToList();
            var comparer = new ClockwiseVector2fComparer(Position);
            Neighbors.Sort((x, y) => comparer.Compare(x.Position, y.Position));
        }
    }
}