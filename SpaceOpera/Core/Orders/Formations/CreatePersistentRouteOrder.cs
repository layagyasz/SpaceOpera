using SpaceOpera.Core.Military;
using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Orders.Formations
{
    public class CreatePersistentRouteOrder : IOrder
    {
        public PersistentRoute Route { get; }

        public CreatePersistentRouteOrder(PersistentRoute route)
        {
            Route = route;
        }

        // Validate against world state.
        public ValidationFailureReason Validate()
        {
            return ValidationFailureReason.None;
        }

        public bool Execute(World world)
        {
            world.Economy.AddPersistentRoute(Route);
            foreach (var fleet in Route.AssignedFleets)
            {
                ((FleetDriver)world.FormationManager.GetDriver(fleet)).SetPersistentRoute(Route);
            }
            return true;
        }
    }
}
