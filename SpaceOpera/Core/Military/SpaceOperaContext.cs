using Cardamom.Graphing.BehaviorTree;

namespace SpaceOpera.Core.Military
{
    public class SpaceOperaContext : SimpleContext
    {
        public class FleetContext : SpaceOperaContext
        {
            public FleetDriver Fleet { get; }

            internal FleetContext(World world, FleetDriver fleet)
                : base(world)
            {
                this.Fleet = fleet;
            }
        }

        public World World { get; }

        public SpaceOperaContext(World world)
        {
            this.World = world;
        }

        public FleetContext ForFleet(FleetDriver fleet)
        {
            return new FleetContext(World, fleet);
        }
    }
}
