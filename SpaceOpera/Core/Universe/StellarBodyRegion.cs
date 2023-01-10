using Cardamom.Utilities;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe
{
    class StellarBodyRegion
    {
        public EventHandler<ElementEventArgs<Division>> OnDivisionAdded { get; set; }

        public string Name { get; private set; }
        public StellarBodySubRegion Center { get; }
        public Biome DominantBiome { get; }
        public StellarBody Parent { get; private set; }
        public List<StellarBodySubRegion> SubRegions { get; }
        public List<ResourceNode> Resources { get; } = new List<ResourceNode>();
        public uint StructureNodes { get; private set; }
        public uint Population { get; private set; }

        public Faction Sovereign { get; private set; }

        public StellarBodyRegion(StellarBodySubRegion Center, IEnumerable<StellarBodySubRegion> SubRegions)
        {
            this.Center = Center;
            this.DominantBiome =
                SubRegions
                .GroupBy(x => x.Biome)
                .Select(x => new KeyValuePair<Biome, int>(x.Key, x.Count()))
                .ArgMax(x => x.Value).Key;
            this.SubRegions = SubRegions.ToList();

            foreach (var region in SubRegions)
            {
                region.OnDivisionAdded += HandleDivisionAdded;
            }
        }

        public void AddPopulation(uint Population)
        {
            this.Population += Population;
        }

        public void AddResources(IEnumerable<ResourceNode> ResourceNodes)
        {
            foreach (var node in ResourceNodes)
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

        public void AddStructureNodes(uint StructureNodes)
        {
            this.StructureNodes += StructureNodes;
        }

        public int GetResourceSize(IMaterial Resource)
        {
            return Resources.FirstOrDefault(x => x.Resource == Resource)?.Size ?? 0;
        }

        public void SetName(string Name)
        {
            this.Name = Name;
        }

        public void SetParent(StellarBody Parent)
        {
            this.Parent = Parent;
        }

        public void SetSovereign(Faction Sovereign)
        {
            this.Sovereign = Sovereign;
        }

        public IEnumerable<StellarBodyRegion> GetNeighbors()
        {
            return SubRegions.SelectMany(x => x.Neighbors).Select(x => x.ParentRegion).Where(x => x != this);
        }

        private void HandleDivisionAdded(object Sender, ElementEventArgs<Division> E)
        {
            OnDivisionAdded?.Invoke(this, E);
        }
    }
}