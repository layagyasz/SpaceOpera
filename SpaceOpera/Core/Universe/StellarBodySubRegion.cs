using Cardamom.Mathematics.Comparers;
using Cardamom.Mathematics.Coordinates;
using OpenTK.Mathematics;
using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Universe
{
    public class StellarBodySubRegion : INavigable
    {
        public EventHandler<ValueEventArgs<Division>>? OnDivisionAdded { get; set; }

        public int Id { get; }
        public string Name => ParentRegion!.Name;
        public NavigableNodeType NavigableNodeType => NavigableNodeType.Ground;
        public Vector3 Center { get; }
        public Biome Biome { get; }

        public StellarBodySubRegion[]? Neighbors { get; private set; }
        public StellarBodyRegion? ParentRegion { get; private set; }

        private readonly List<Division> _divisions = new();

        public StellarBodySubRegion(int id, Vector3 center, Biome biome)
        {
            Id = id;
            Center = center;
            Biome = biome;
        }

        public void SetParentRegion(StellarBodyRegion region)
        {
            ParentRegion = region;
        }

        public void SetNeighbors(IEnumerable<StellarBodySubRegion> neighbors)
        {
            Neighbors = neighbors.ToArray();
            var comparer = new ClockwiseVector3Comparer(Center, Center, Neighbors[0].Center - Center);
            Array.Sort(Neighbors, (x, y) => comparer.Compare(x.Center, y.Center));
        }

        public void AddDivision(Division division)
        {
            _divisions.Add(division);
            OnDivisionAdded?.Invoke(this, new ValueEventArgs<Division>(division));
        }

        public void RemoveDivision(Division division)
        {
            _divisions.Remove(division);
        }

        public override string ToString()
        {
            return string.Format(
                "[StellarBodySubRegion: Id={0}, Neighbors={1}]",
                Id, 
                string.Join(",", Neighbors?.Select(x => x.Id) ?? Enumerable.Empty<int>()));
        }
    }
}