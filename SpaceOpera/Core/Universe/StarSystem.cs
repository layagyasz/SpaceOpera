using Cardamom.Planar;
using SFML.System;
using SFML.Window;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Voronoi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe
{
    class StarSystem
    {
        public string Name { get; private set; }
        public Vector2f Position { get; }
        public Star Star { get; }
        public float InnerBoundary { get; }
        public float OuterBoundary { get; }
        public float TransitLimit { get; }
        public List<StellarBody> Orbiters { get; }
        public List<SolarOrbitRegion> OrbitalRegions { get; }
        public List<StarSystem> Neighbors { get; private set; }
        public SortedList<double, TransitRegion> Transits { get; private set; }

        public StarSystem(
            Vector2f Position,
            Star Star, 
            float InnerBoundary,
            float OuterBoundary, 
            float TransitLimit,
            IEnumerable<StellarBody> Orbiters)
        {
            this.Position = Position;
            this.Star = Star;
            this.InnerBoundary = InnerBoundary;
            this.OuterBoundary = OuterBoundary;
            this.TransitLimit = TransitLimit;
            this.Orbiters = Orbiters.ToList();
            this.OrbitalRegions = Orbiters.Select(x => new SolarOrbitRegion(new LocalOrbitRegion(x))).ToList();
            this.Transits = new SortedList<double, TransitRegion>();
        }

        public void AddTransit(StarSystem Neighbor)
        {
            if (!Neighbors.Contains(Neighbor))
            {
                throw new ArgumentException(string.Format("{0} is not a neighbor of {1}", Neighbor, this));
            }
            Transits.Add(MathUtils.Theta(Position, Neighbor.Position), new TransitRegion(Neighbor));
        }

        public bool ContainsFaction(Faction Faction)
        {
            return Orbiters.Any(x => x.ContainsFaction(Faction));
        }

        public void SetName(string Name)
        {
            this.Name = Name;
        }

        public void SetNeighbors(IEnumerable<StarSystem> Neighbors)
        {
            this.Neighbors = Neighbors.ToList();

            var comparer = new ClockwiseVector2fComparer(Position);
            this.Neighbors.Sort((x, y) => comparer.Compare(x.Position, y.Position));
        }
    }
}