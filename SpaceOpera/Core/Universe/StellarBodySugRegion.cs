using Cardamom.Mathematics.Coordinates;
using OpenTK.Mathematics;
using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Universe
{
    public class StellarBodySubRegion : INavigable
    {
        public EventHandler<ElementEventArgs<Division>>? OnDivisionAdded { get; set; }

        public int Id { get; }
        public string Name => ParentRegion!.Name;
        public NavigableNodeType NavigableNodeType => NavigableNodeType.Ground;
        public Vector3 Center { get; }
        public Spherical3 SphericalCenter { get; }
        public Biome Biome { get; }

        public Vector3[]? Bounds { get; private set; }
        public List<StellarBodySubRegion>? Neighbors { get; private set; }
        public StellarBodyRegion? ParentRegion { get; private set; }

        private readonly List<Division> _divisions = new();

        public StellarBodySubRegion(int id, Vector3 center, Spherical3 sphericalCenter, Biome biome)
        {
            Id = id;
            Center = center;
            SphericalCenter = sphericalCenter;
            Biome = biome;
        }

        public void SetParentRegion(StellarBodyRegion region)
        {
            ParentRegion = region;
        }

        public void SetNeighbors(IEnumerable<StellarBodySubRegion> neighbors)
        {
            Neighbors = neighbors.ToList();

            var comparer = new ClockwiseSurface3dComparer(SphericalCenter);
            Neighbors.Sort((x, y) => comparer.Compare(x.Center, y.Center));

            Bounds = new Vector3[Neighbors.Count];
            for (int i = 0; i < Neighbors.Count; ++i)
            {
                Bounds[i] = 
                    SphericalCenter.Radius 
                    * ((Center + Neighbors[i].Center 
                    + Neighbors[(i + 1) % Neighbors.Count].Center) / 3).Normalize();
            }
        }

        public void AddDivision(Division division)
        {
            _divisions.Add(division);
            OnDivisionAdded?.Invoke(this, new ElementEventArgs<Division>(division));
        }

        public void RemoveDivision(Division division)
        {
            _divisions.Remove(division);
        }

        public override string ToString()
        {
            return string.Format("[StellarBodySubRegion: Id={0}]", Id);
        }
    }
}