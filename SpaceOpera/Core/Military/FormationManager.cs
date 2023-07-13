using SpaceOpera.Core.Military.Ai;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Military
{
    public class FormationManager
    {
        private readonly Dictionary<IFormation, IFormationDriver> _drivers = new();

        public void AddArmy(Army army)
        {
            _drivers.Add(army, new ArmyDriver(army, army.Divisions.Select(x => (DivisionDriver)_drivers[x])));
        }

        public void AddDivision(Division division)
        {
            _drivers.Add(division, new DivisionDriver(division));
        }

        public void AddFleet(Fleet fleet)
        {
            _drivers.Add(fleet, new FleetDriver(fleet));
        }

        public IEnumerable<ArmyDriver> GetArmyDrivers()
        {
            return _drivers.Values.Where(x => x is ArmyDriver).Cast<ArmyDriver>();
        }

        public IEnumerable<ArmyDriver> GetArmyDriversFor(Faction faction)
        {
            return GetArmyDrivers().Where(x => x.Formation.Faction == faction);
        }

        public IEnumerable<AtomicFormationDriver> GetAtomicDrivers()
        {
            return _drivers.Values.Where(x => x is AtomicFormationDriver).Cast<AtomicFormationDriver>();
        }

        public IEnumerable<DivisionDriver> GetDivisionDrivers()
        {
            return _drivers.Values.Where(x => x is DivisionDriver).Cast<DivisionDriver>();
        }

        public IEnumerable<DivisionDriver> GetDivisionDriversFor(Faction faction)
        {
            return GetDivisionDrivers().Where(x => x.Formation.Faction == faction);
        }

        public IFormationDriver GetDriver(IFormation formation)
        {
            return _drivers[formation];
        }

        public IEnumerable<FleetDriver> GetFleetDrivers()
        {
            return _drivers.Values.Where(x => x is FleetDriver).Cast<FleetDriver>();
        }

        public IEnumerable<FleetDriver> GetFleetDriversFor(Faction faction)
        {
            return GetFleetDrivers().Where(x => x.Formation.Faction == faction);
        }

        public void Tick(World world)
        {
            var context = new SpaceOperaContext(world);
            foreach (var driver in _drivers.Values)
            {
                driver.Tick(context);
            }
        }
    }
}