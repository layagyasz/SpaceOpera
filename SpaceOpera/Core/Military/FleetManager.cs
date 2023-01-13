namespace SpaceOpera.Core.Military
{
    public class FleetManager
    {
        private readonly Dictionary<Fleet, FleetDriver> _drivers = new();

        public void AddFleet(Fleet fleet)
        {
            _drivers.Add(fleet, new FleetDriver(fleet));
        }

        public FleetDriver GetDriver(Fleet fleet)
        {
            return _drivers[fleet];
        }

        public void Tick(World world)
        {
            var context = new SpaceOperaContext(world);
            foreach (var fleet in _drivers.Values)
            {
                fleet.Tick(context);
            }
        }
    }
}