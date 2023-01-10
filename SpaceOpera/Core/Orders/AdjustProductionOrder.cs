using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Orders
{
    class AdjustProductionOrder : IImmediateOrder
    {
        public StellarBodyRegionHolding Holding { get; }
        public Structure Structure { get; }
        public MultiCount<Recipe> Production { get; }

        public AdjustProductionOrder(
            StellarBodyRegionHolding Holding, Structure Structure, MultiCount<Recipe> Production)
        {
            this.Holding = Holding;
            this.Structure = Structure;
            this.Production = Production;
        }

        public ValidationFailureReason Validate()
        {
            int totalUsage = 0;
            foreach (var production in Production)
            {
                int productionUsage = production.Value + Holding.GetProduction(production.Key);
                totalUsage += productionUsage;
                if (productionUsage < 0)
                {
                    return ValidationFailureReason.ILLEGAL_ORDER;
                }
                if (production.Value >  Holding.GetAvailableResourceNodes(production.Key.BoundResourceNode))
                {
                    return ValidationFailureReason.TOO_FEW_RESOURCE_NODES;
                }
            }
            return totalUsage <= Holding.GetStructureCount(Structure)
                ? ValidationFailureReason.NONE
                : ValidationFailureReason.TOO_FEW_STRUCTURES;
        }

        public bool Execute(World World)
        {
            Holding.AdjustProduction(Production);
            return true;
        }
    }
}