using SpaceOpera.Core.Military;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Orders.Formations.Assignments
{
    public class CreatePersistentRouteOrder : IOrder
    {
        public Faction Faction { get; set; }
        public PersistentRoute Route { get; }

        public CreatePersistentRouteOrder(Faction faction, PersistentRoute route)
        {
            Faction = faction;
            Route = route;
        }

        public ValidationFailureReason Validate(World world)
        {
            if (Route.Faction != Faction || Route.AssignedFleets.Any(x => x.Faction != Faction))
            {
                return ValidationFailureReason.IllegalOrder;
            }
            if (Route.LeftMaterials.Count == 0 || Route.RightMaterials.Count == 0)
            {
                return ValidationFailureReason.InvalidRoute;
            }
            foreach (var existingRoute in world.Economy.GetPersistentRoutesFor(Faction))
            {
                if (existingRoute.AssignedFleets.Intersect(Route.AssignedFleets).Any())
                {
                    return ValidationFailureReason.FormationAlreadyAssigned;
                }
            }
            return ValidationFailureReason.None;
        }

        public bool Execute(World world)
        {
            world.Economy.AddPersistentRoute(Route);
            foreach (var fleet in Route.AssignedFleets)
            {
                ((FleetDriver)world.Formations.GetDriver(fleet)).SetPersistentRoute(Route);
            }
            return true;
        }
    }
}
