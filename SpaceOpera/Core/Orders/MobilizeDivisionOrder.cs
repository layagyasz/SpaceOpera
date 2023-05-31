using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;
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

        public ValidationFailureReason Validate(World world)
        {
            return Division.StellarBodyLocation == null 
                ? ValidationFailureReason.IllegalOrder : ValidationFailureReason.None;
        }

        public bool Execute(World world)
        {
            world.AddProject(new MobilizeDivisionProject(Holding, Division));
            return true;
        }
    }
}