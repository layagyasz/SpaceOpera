using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Orders
{
    class MobilizeDivisionOrder : TimedOrder, IProject
    {
        public StellarBodyRegionHolding Holding { get; }
        public Division Division { get; }

        public MobilizeDivisionOrder(StellarBodyRegionHolding Holding, Division Division)
            : base(1)
        {
            this.Holding = Holding;
            this.Division = Division;
        }

        public override ValidationFailureReason Validate()
        {
            return Division.StellarBodyLocation == null 
                ? ValidationFailureReason.ILLEGAL_ORDER : ValidationFailureReason.NONE;
        }

        public override void Setup()
        {
            Holding.AddProject(this);
        }

        public override void Cleanup()
        {
            Holding.RemoveProject(this);
            // TODO: Reimplement
            // Division.SetLocation(Holding.Region.Center);
        }
    }
}