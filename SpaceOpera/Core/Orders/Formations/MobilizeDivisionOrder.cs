using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Orders.Formations
{
    public class MobilizeDivisionOrder : IOrder
    {
        public EconomicSubzoneHolding Holding { get; }
        public Division Division { get; }

        public MobilizeDivisionOrder(EconomicSubzoneHolding holding, Division division)
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
            world.Projects.Add(new MobilizeDivisionProject(Holding, Division));
            return true;
        }
    }
}