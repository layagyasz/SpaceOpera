using SpaceOpera.Core.Military.Ai;

namespace SpaceOpera.Core.Military
{
    public class FleetDriver : AtomicFormationDriver
    {
        public FleetDriver(Fleet fleet)
            : base(fleet, new FleetAi()) { }
    }
}
