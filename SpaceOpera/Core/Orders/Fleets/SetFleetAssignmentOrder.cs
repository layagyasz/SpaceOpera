using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Orders.Fleets
{
    public class SetFleetAssignmentOrder : IOrder
    {
        public FleetDriver Fleet { get; }
        public FleetAssignment Assignment { get; }

        public SetFleetAssignmentOrder(FleetDriver fleet, FleetAssignment assignment)
        {
            Fleet = fleet;
            Assignment = assignment;
        }

        public ValidationFailureReason Validate()
        {
            return ValidationFailureReason.None;
        }

        public bool Execute(World world)
        {
            Fleet.SetAssignment(Assignment);
            return true;
        }
    }
}