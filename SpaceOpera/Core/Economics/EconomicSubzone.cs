using Cardamom.Trackers;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics
{
    public class EconomicSubzone : ProjectHub
    {
        public EconomicZone Parent { get; set; }

        private readonly IntPool _structureNodes = new(0);
        private readonly MultiCount<Structure> _structures = new();
        private readonly Dictionary<IMaterial, IntPool> _resourceNodes = new();
        private readonly MultiCount<Recipe> _production = new();

        public EconomicSubzone(EconomicZone parent)
        {
            Parent = parent;
        }

        public void AddResourceNodes(Count<ResourceNode> nodes)
        {
            if (_resourceNodes.TryGetValue(nodes.Key.Resource, out var node))
            {
                node.ChangeMax(nodes.Value);
            }
            else
            {
                _resourceNodes.Add(nodes.Key.Resource, new(nodes.Value));
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

        public void ReserveStructureNodes(Count<Structure> construction)
        {
            _structureNodes.Change(construction.Value);
        }

        public void ReleaseStructureNodes(Count<Structure> construction)
        {
            _structureNodes.Change(-construction.Value);
        }
    }
}
