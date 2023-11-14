using Cardamom.Collections;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Universe
{
    public class StellarBodyRegion
    {
        public string Name { get; private set; } = string.Empty;
        public StellarBodySubRegion Center { get; }
        public Biome DominantBiome { get; }
        public StellarBody? Parent { get; private set; }
        public List<StellarBodySubRegion> SubRegions { get; }
        public List<ResourceNode> Resources { get; } = new();
        public uint StructureNodes { get; private set; }
        public uint Population { get; private set; }

        public Faction? Sovereign { get; private set; }

        public StellarBodyRegion(StellarBodySubRegion center, IEnumerable<StellarBodySubRegion> subRegions)
        {
            Center = center;
            DominantBiome =
                subRegions
                    .GroupBy(x => x.Biome)
                    .Select(x => new KeyValuePair<Biome, int>(x.Key, x.Count()))
                    .ArgMax(x => x.Value)!.Key;
            SubRegions = subRegions.ToList();
        }

        public void AddPopulation(uint population)
        {
            Population += population;
            Parent!.Population.Invalidate();
        }

        public void AddResources(IEnumerable<ResourceNode> resourceNodes)
        {
            foreach (var node in resourceNodes)
            {
                var currentNode = Resources.FirstOrDefault(x => node.Resource == x.Resource);
                if (currentNode == null)
                {
                    Resources.Add(node);
                }
                else
                {
                    currentNode.Combine(node);
                }
            }
        }

        public void AddStructureNodes(uint structureNodes)
        {
            StructureNodes += structureNodes;
        }

        public void ChangeOccupation()
        {
            Parent!.ChangeOccupation();
        }

        public int GetRegionCount(bool isTraversable)
        {
            return DominantBiome.IsTraversable == isTraversable ? SubRegions.Count : 0;
        }

        public int GetResourceSize(IMaterial resource)
        {
            return Resources.FirstOrDefault(x => x.Resource == resource)?.Size ?? 0;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetParent(StellarBody parent)
        {
            Parent = parent;
        }

        public void SetSovereign(Faction sovereign)
        {
            Sovereign = sovereign;
        }

        public IEnumerable<StellarBodyRegion> GetNeighbors()
        {
            return SubRegions
                .SelectMany(x => x.Neighbors!).Select(x => x.ParentRegion!).Where(x => x != this).Distinct();
        }
    }
}