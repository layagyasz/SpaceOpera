using Cardamom.Mathematics.Comparers;
using OpenTK.Mathematics;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Universe
{
    public class StellarBodySubRegion : INavigable
    {
        public int Id { get; }
        public string Name => ParentRegion!.Name;
        public NavigableNodeType NavigableNodeType => NavigableNodeType.Ground;
        public Vector3 Center { get; }
        public Biome Biome { get; }

        public StellarBodySubRegion[]? Neighbors { get; private set; }
        public StellarBodyRegion? ParentRegion { get; private set; }
        public Faction? Occupation { get; private set; }

        public StellarBodySubRegion(int id, Vector3 center, Biome biome)
        {
            Id = id;
            Center = center;
            Biome = biome;
        }

        public void Enter(Faction faction)
        {
            SetOccupation(faction);
        }

        public void SetOccupation(Faction faction)
        {
            if (faction == ParentRegion!.Sovereign)
            {
                Occupation = null;
                ParentRegion!.ChangeOccupation();
            }
            else if (Occupation != faction)
            {
                Occupation = faction;
                ParentRegion!.ChangeOccupation();
            }
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

        public override string ToString()
        {
            return string.Format(
                "[StellarBodySubRegion: Id={0}, Neighbors={1}]",
                Id, 
                string.Join(",", Neighbors?.Select(x => x.Id) ?? Enumerable.Empty<int>()));
        }
    }
}