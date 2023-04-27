using SpaceOpera.Core.Military.Ai;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Military
{
    public class FormationManager
    {
        private readonly List<Army> _armies = new();
        private readonly Dictionary<IFormation, FormationDriver> _drivers = new();

        public void AddArmy(Army army)
        {
            _armies.Add(army);
        }

        public void AddDivision(Division division)
        {
            _drivers.Add(division, new DivisionDriver(division));
        }

        public void AddFleet(Fleet fleet)
        {
            _drivers.Add(fleet, new FleetDriver(fleet));
        }

        public IEnumerable<FormationDriver> GetDrivers()
        {
            return _drivers.Values;
        }

        public IEnumerable<FormationDriver> GetDivisionDrivers()
        {
            return GetDrivers().Where(x => x is DivisionDriver);
        }

        public IEnumerable<FormationDriver> GetDivisionDriversFor(Faction faction)
        {
            return GetDivisionDrivers().Where(x => x.Formation.Faction == faction);
        }

        public IEnumerable<FormationDriver> GetFleetDrivers()
        {
            return GetDrivers().Where(x => x is FleetDriver);
        }

        public IEnumerable<FormationDriver> GetFleetDriversFor(Faction faction)
        {
            return GetFleetDrivers().Where(x => x.Formation.Faction == faction);
        }

        public FormationDriver GetDriver(IFormation formation)
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