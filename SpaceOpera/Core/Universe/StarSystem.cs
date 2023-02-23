using Cardamom.Mathematics;
using Cardamom.Mathematics.Comparers;
using OpenTK.Mathematics;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Universe
{
    public class StarSystem
    {
        public string Name { get; private set; } = string.Empty;
        public Vector3 Position { get; }
        public Star Star { get; }
        public Interval ViableRange { get; }
        public Interval GoldilocksRange { get; }
        public float TransitLimit { get; }
        public List<StellarBody> Orbiters { get; }
        public List<SolarOrbitRegion> OrbitalRegions { get; }
        public List<StarSystem>? Neighbors { get; private set; }
        public SortedList<float, TransitRegion> Transits { get; private set; }

        public StarSystem(
            Vector3 position,
            Star star, 
            Interval viableRange,
            Interval goldilocksRange,
            float transitLimit,
            IEnumerable<StellarBody> orbiters)
        {
            Position = position;
            Star = star;
            ViableRange = viableRange;
            GoldilocksRange = goldilocksRange;
            TransitLimit = transitLimit;
            Orbiters = orbiters.ToList();
            OrbitalRegions = orbiters.Select(x => new SolarOrbitRegion(new(x))).ToList();
            Transits = new();
        }

        public void AddTransit(StarSystem neighbor)
        {
            if (!Neighbors!.Contains(neighbor))
            {
                throw new ArgumentException(string.Format("{0} is not a neighbor of {1}", neighbor, this));
            }
            Transits.Add(MathUtils.Theta(Position.Xz, neighbor.Position.Xz), new(neighbor));
        }

        public bool ContainsFaction(Faction faction)
        {
            return Orbiters.Any(x => x.ContainsFaction(faction));
        }

        public int GetSize()
        {
            return Transits.Count + 2 * OrbitalRegions.Count + Orbiters.Sum(x => x.GetSize());
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetNeighbors(IEnumerable<StarSystem> neighbors)
        {
            Neighbors = neighbors.ToList();
            var comparer = new ClockwiseVector2Comparer(Position.Xz);
            Neighbors.Sort((x, y) => comparer.Compare(x.Position.Xz, y.Position.Xz));
        }
    }
}