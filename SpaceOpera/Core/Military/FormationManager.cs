namespace SpaceOpera.Core.Military
{
    public class FormationManager
    {
        private readonly Dictionary<IFormation, IFormationDriver> _drivers = new();

        public void AddFleet(Fleet fleet)
        {
            _drivers.Add(fleet, new FleetDriver(fleet));
        }

        public IEnumerable<IFormationDriver> GetDrivers()
        {
            return _drivers.Values;
        }

        public IFormationDriver GetDriver(IFormation formation)
        {
            return _drivers[formation];
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