using Cardamom.Trackers;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics
{
    public class EconomicSubzoneHolding : BaseProjectHub
    {
        public string Name => Region.Name;
        public Faction Owner => Parent.Parent.Owner;
        public EconomicZoneHolding Parent { get; set; }
        public StellarBodyRegion Region { get; }

        private readonly IntPool _structureNodes = new(0);
        private readonly MultiCount<Structure> _structures = new();
        private readonly Dictionary<IMaterial, IntPool> _resourceNodes = new();
        private readonly MultiCount<Recipe> _production = new();
        private readonly List<Division> _divisions = new();

        public EconomicSubzoneHolding(EconomicZoneHolding parent, StellarBodyRegion region)
        {
            Parent = parent;
            Region = region;
        }

        public void AddDivision(Division division)
        {
            _divisions.Add(division);
        }

        public void AddResourceNodes(Count<ResourceNode> nodes)
        {
            if (_resourceNodes.TryGetValue(nodes.Key.Resource, out var node))
            {
                node.ChangeMax(nodes.Value);
            }
            else
            {
                _resourceNodes.Add(nodes.Key.Resource, new(nodes.Value, /* startFull= */ false));
            }
        }

        public void AddStructures(Count<Structure> structures)
        {
            _structureNodes.Change(structures.Value);
            _structures.Add(structures);
        }

        public void AddStructureNodes(int nodes)
        {
            _structureNodes.ChangeMax(nodes);
        }

        public void AdjustProduction(MultiCount<Recipe> production)
        {
            _production.Add(production);
            Parent.AdjustProduction(production);
        }

        public int GetAvailableResourceNodes(IMaterial? resource)
        {
            if (resource == null)
            {
                return int.MaxValue;
            }
            if (_resourceNodes.TryGetValue(resource, out var node))
            {
                return node.Remaining;
            }
            return 0;
        }

        public IEnumerable<Division> GetDivisions()
        {
            return _divisions;
        }

        public long GetPopulation()
        {
            return Region.Population;
        }

        public int GetRegionCount(bool isTraversable)
        {
            return Region.GetRegionCount(isTraversable);
        }

        public IEnumerable<IMaterial> GetResources()
        {
            return _resourceNodes.Keys;
        }

        public int GetResourceNodes(IMaterial? resource)
        {
            if (resource == null)
            {
                return int.MaxValue;
            }
            if (_resourceNodes.TryGetValue(resource, out var node))
            {
                return node.MaxAmount;
            }
            return 0;
        }

        public int GetAvailableStructureNodes()
        {
            return _structureNodes.Remaining;
        }

        public int GetProduction(Recipe recipe)
        {
            return _production[recipe];
        }

        public int GetStructureCount(Structure structure)
        {
            return _structures[structure];
        }

        public int GetStructureNodes()
        {
            return _structureNodes.MaxAmount;
        }

        public void ReserveStructureNodes(Count<Structure> construction)
        {
            _structureNodes.Change(construction.Value);
        }

        public void ReleaseStructureNodes(Count<Structure> construction)
        {
            _structureNodes.Change(-construction.Value);
        }

        public void RemoveDivision(Division division)
        {
            _divisions.Remove(division);
        }
    }
}
