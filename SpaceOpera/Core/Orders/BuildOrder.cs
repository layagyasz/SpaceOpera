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
    class BuildOrder : ICompoundOrder
    {
        public StellarBodyRegionHolding Holding { get; }
        public MultiCount<Structure> Structures { get; }

        public BuildOrder(StellarBodyRegionHolding Holding, MultiCount<Structure> Structures)
        {
            this.Holding = Holding;
            this.Structures = Structures;
        }

        public ValidationFailureReason Validate()
        {
            foreach (var construction in Structures)
            {
                if (construction.Value < 0)
                {
                    return ValidationFailureReason.ILLEGAL_ORDER;
                }
            }
            return Structures.GetTotal() <= Holding.GetAvailableStructureNodes() 
                ? ValidationFailureReason.NONE 
                : ValidationFailureReason.TOO_FEW_STRUCTURE_NODES;
        }

        public IEnumerable<IOrder> GetChildOrders()
        {
            foreach (var construction in Structures)
            {
                yield return new SingleBuildOrder(Holding, new Count<Structure>(construction.Key, construction.Value));
            }
        }
    }
}