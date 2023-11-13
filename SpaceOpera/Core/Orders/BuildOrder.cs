using Cardamom.Trackers;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;

namespace SpaceOpera.Core.Orders
{
    class BuildOrder : IOrder
    {
        public EconomicSubzoneHolding Holding { get; }
        public MultiCount<Structure> Structures { get; }

        public BuildOrder(EconomicSubzoneHolding holding, MultiCount<Structure> structures)
        {
            Holding = holding;
            Structures = structures;
        }

        public MultiQuantity<IMaterial> GetTotalCost()
        {
            MultiQuantity<IMaterial> cost = new();
            foreach (var structure in Structures)
            {
                cost.Add(structure.Value * structure.Key.Cost);
            }
            return cost;
        }

        public ValidationFailureReason Validate(World world)
        {
            foreach (var construction in Structures)
            {
                if (construction.Value < 0)
                {
                    return ValidationFailureReason.IllegalOrder;
                }
            }
            return Structures.GetTotal() <= Holding.GetAvailableStructureNodes() 
                ? ValidationFailureReason.None 
                : ValidationFailureReason.TooFewStructureNodes;
        }

        public bool Execute(World world)
        {
            foreach (var construction in Structures.GetCounts())
            {
                world.Projects.Add(new BuildProject(Holding, construction));
            }
            return true;
        }
    }
}