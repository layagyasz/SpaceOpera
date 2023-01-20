using SpaceOpera.Core.Military;
using SpaceOpera.Core.Orders;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Languages
{
    public class SetFleetActiveRegionOrder : IOrder
    {
        public FleetDriver Fleet { get; }
        public ISet<INavigable> ActiveRegion { get; }

        public SetFleetActiveRegionOrder(FleetDriver fleet, ISet<INavigable> activeRegion)
        {
            Fleet = fleet;
            ActiveRegion = activeRegion;
        }

        public ValidationFailureReason Validate()
        {
            return ValidationFailureReason.None;
        }

        public bool Execute(World world)
        {
            Fleet.SetActiveRegion(ActiveRegion);
            return true;
        }
    }
}