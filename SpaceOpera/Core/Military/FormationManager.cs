using SpaceOpera.Core.Military.Ai;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Military
{
    public class FormationManager
    {
        private readonly Dictionary<IFormation, ArmyDriver> _metaDrivers = new();
        private readonly Dictionary<IFormation, AtomicFormationDriver> _atomicDrivers = new();

        public void AddArmy(Army army)
        {
            _metaDrivers.Add(army, new ArmyDriver(army));
        }

        public void AddDivision(Division division)
        {
            _atomicDrivers.Add(division, new DivisionDriver(division));
        }

        public void AddFleet(Fleet fleet)
        {
            _atomicDrivers.Add(fleet, new FleetDriver(fleet));
        }

        public IEnumerable<ArmyDriver> GetArmies()
        {
            return _metaDrivers.Values;
        }

        public IEnumerable<ArmyDriver> GetArmiesFor(Faction faction)
        {
            return GetArmies().Where(x => x.Formation.Faction == faction);
        }

        public IEnumerable<AtomicFormationDriver> GetAtomicDrivers()
        {
            return _atomicDrivers.Values;
        }

        public IEnumerable<AtomicFormationDriver> GetDivisionDrivers()
        {
            return GetAtomicDrivers().Where(x => x is DivisionDriver);
        }

        public IEnumerable<AtomicFormationDriver> GetDivisionDriversFor(Faction faction)
        {
            return GetDivisionDrivers().Where(x => x.Formation.Faction == faction);
        }

        public IEnumerable<AtomicFormationDriver> GetFleetDrivers()
        {
            return GetAtomicDrivers().Where(x => x is FleetDriver);
        }

        public IEnumerable<AtomicFormationDriver> GetFleetDriversFor(Faction faction)
        {
            return GetFleetDrivers().Where(x => x.Formation.Faction == faction);
        }

        public AtomicFormationDriver GetAtomicDriver(IAtomicFormation formation)
        {
            return _atomicDrivers[formation];
        }

        public void Tick(World world)
        {
            var context = new SpaceOperaContext(world);
            foreach (var fleet in _atomicDrivers.Values)
            {
                fleet.Tick(context);
            }
        }
    }
}