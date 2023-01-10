using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Orders
{
    class SingleBuildOrder : ResourcedOrder, IProject
    {
        public StellarBodyRegionHolding Holding { get; }
        public Count<Structure> Construction { get; }

        public SingleBuildOrder(StellarBodyRegionHolding Holding, Count<Structure> Construction)
            : base(Construction.Value.BuildTime, Construction.Value.Cost, Holding.Parent)
        {
            this.Holding = Holding;
            this.Construction = Construction;
        }

        public override ValidationFailureReason Validate()
        {
            if (Construction.Amount < 0)
            {
                return ValidationFailureReason.ILLEGAL_ORDER;
            }
            return Construction.Amount <= Holding.GetAvailableStructureNodes()
                ? ValidationFailureReason.NONE
                : ValidationFailureReason.TOO_FEW_STRUCTURE_NODES;
        }

        public override void Setup()
        {
            Holding.AddProject(this);
            Holding.ReserveStructureNodes(Construction);
        }

        public override void Cleanup()
        {
            Holding.RemoveProject(this);
            Holding.ReleaseStructureNodes(Construction);
            if (!Cancelled)
            {
                Holding.AddStructures(Construction);
            }
        }
    }
}