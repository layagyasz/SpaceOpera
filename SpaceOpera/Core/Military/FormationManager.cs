using SpaceOpera.Core.Events;
using SpaceOpera.Core.Military.Ai;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Military
{
    public class FormationManager
    {
        public EventHandler<IFormationDriver>? Created { get; set; }
        public EventHandler<MovementEventArgs>? Moved { get; set; }
        public EventHandler<IFormationDriver>? Removed { get; set; }

        private Dictionary<IFormation, IFormationDriver> _drivers = new();

        public ArmyDriver AddArmy(Army army)
        {
            var driver = new ArmyDriver(army);
            Add(driver);
            return driver;
        }

        public DivisionDriver AddDivision(Division division)
        {
            var driver = new DivisionDriver(division);
            Add(driver);
            return driver;
        }

        public FleetDriver AddFleet(Fleet fleet)
        {
            var driver = new FleetDriver(fleet);
            Add(driver);
            return driver;
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

        public IEnumerable<IFormationDriver> GetDrivers(Faction faction)
        {
            return _drivers.Values.Where(x => x.Formation.Faction == faction);
        }

        public IEnumerable<FleetDriver> GetFleetDrivers()
        {
            return _drivers.Values.Where(x => x is FleetDriver).Cast<FleetDriver>();
        }

        public IEnumerable<FleetDriver> GetFleetDriversFor(Faction faction)
        {
            return GetFleetDrivers().Where(x => x.Formation.Faction == faction);
        }

        public float GetGroundForcePower(Faction faction)
        {
            return GetDivisionDriversFor(faction).Sum(x => x.Formation.GetMilitaryPower());
        }

        public float GetFleetPower(Faction faction)
        {
            return GetFleetDriversFor(faction).Sum(x => x.Formation.GetMilitaryPower());
        }

        public void Tick(World world)
        {
            lock (_drivers)
            {
                var context = new SpaceOperaContext(world);
                var driversCopy = new Dictionary<IFormation, IFormationDriver>();
                foreach (var driver in _drivers.Values)
                {
                    if (driver.Formation.IsDestroyed())
                    {
                        if (driver is FleetDriver || driver is ArmyDriver)
                        {
                            world.Events.Add(new FormationDestroyedEvent(driver.Formation));
                        }
                        Removed?.Invoke(this, driver);
                    }
                    else
                    {
                        driver.Tick(context);
                        driversCopy.Add(driver.Formation, driver);
                    }
                }
                _drivers = driversCopy;
            }
        }

        private void Add(IFormationDriver driver)
        {
            lock (_drivers)
            {
                _drivers.Add(driver.Formation, driver);
            }
            if (driver is AtomicFormationDriver atomic)
            {
                atomic.Moved += HandleMove;
            }
            Created?.Invoke(this, driver);
        }

        private void HandleMove(object? sender, MovementEventArgs e)
        {
            Moved?.Invoke(sender, e);
        }
    }
}