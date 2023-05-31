using Cardamom.Trackers;
using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Orders
{
    class AdjustProductionOrder : IOrder
    {
        public StellarBodyRegionHolding Holding { get; }
        public Structure Structure { get; }
        public MultiCount<Recipe> Production { get; }

        public AdjustProductionOrder(
            StellarBodyRegionHolding holding, Structure structure, MultiCount<Recipe> production)
        {
            Holding = holding;
            Structure = structure;
            Production = production;
        }

        public ValidationFailureReason Validate(World world)
        {
            int totalUsage = 0;
            foreach (var production in Production)
            {
                int productionUsage = production.Value + Holding.GetProduction(production.Key);
                totalUsage += productionUsage;
                if (productionUsage < 0)
                {
                    return ValidationFailureReason.IllegalOrder;
                }
                if (productionUsage >  Holding.GetAvailableResourceNodes(production.Key.BoundResourceNode))
                {
                    return ValidationFailureReason.TooFewResourceNodes;
                }
            }
            return totalUsage <= Holding.GetStructureCount(Structure)
                ? ValidationFailureReason.None
                : ValidationFailureReason.TooFewStructures;
        }

        public bool Execute(World World)
        {
            Holding.AdjustProduction(Production);
            return true;
        }
    }
}