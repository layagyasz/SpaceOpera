using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Orders
{
    public class MobilizeDivisionOrder : IOrder
    {
        public StellarBodyRegionHolding Holding { get; }
        public Division Division { get; }

        public MobilizeDivisionOrder(StellarBodyRegionHolding holding, Division division)
        {
            Holding = holding;
            Division = division;
        }

        public ValidationFailureReason Validate()
        {
            return Division.StellarBodyLocation == null 
                ? ValidationFailureReason.IllegalOrder : ValidationFailureReason.None;
        }

        public bool Execute(World world)
        {
            world.AddProject(new MobilizeDivisionOrder(Holding, Division));
            return true;
        }
    }
}