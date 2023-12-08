using Cardamom.Trackers;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military
{
    public abstract class BaseAtomicFormation : IAtomicFormation
    {
        private static readonly float s_BaseRecoherence = 0.1f;

        public EventHandler<MovementEventArgs>? Moved { get; set; }

        public string Name { get; private set; } = string.Empty;
        public Faction Faction { get; }
        public INavigable? Position { get; private set; }
        public IPool Health { get; }
        public Pool Cohesion { get; } = new(1);
        public List<UnitGrouping> Composition { get; } = new();
        public Inventory Inventory { get; } = new(0);
        public bool InCombat { get; private set; }

        private bool _isDestructable;

        protected BaseAtomicFormation(Faction faction)
        {
            Faction = faction;
            Health =
                new VirtualPool(
                    () => Composition.Sum(x => x.Count.Amount), () => Composition.Sum(x => x.Count.MaxAmount));
        }

        public void Add(UnitGrouping unitGrouping)
        {
            var currentGrouping = Composition.Where(x => x.Unit == unitGrouping.Unit).FirstOrDefault();
            if (currentGrouping == null)
            {
                Composition.Add(unitGrouping);
            }
            else {
                currentGrouping.Combine(unitGrouping);
            }
            Inventory.SetSize(Inventory.Size + unitGrouping.Unit.CargoSpace * unitGrouping.Count.Amount);
            _isDestructable = true;
        }

        public void Add(Count<Unit> unit)
        {
            Add(new UnitGrouping(unit));
        }

        public void Add(MultiCount<Unit> units)
        {
            foreach (var unit in units.GetCounts())
            {
                Add(unit);
            }
        }

        public void Cohere()
        {
            Cohesion.Change((InCombat ? .1f : 1) * s_BaseRecoherence);
        }

        public void CheckInventory()
        {
            Inventory.SetSize(Composition.Sum(x => x.Unit.CargoSpace * x.Count.Amount));
        }

        public void EnterCombat()
        {
            InCombat = true;
        }

        public void ExitCombat()
        {
            InCombat = false;
        }

        public float GetCommand()
        {
            return Composition.Sum(x => x.Unit.Command * x.Count.Amount);
        }

        public float GetDetection()
        {
            return Composition.Max(x => x.Unit.Detection.UnitValue);
        }

        public float GetEvasion()
        {
            return Composition.Min(x => x.Unit.Evasion.UnitValue);
        }

        public float GetSpeed(NavigableEdgeType type)
        {
            return type switch
            {
                NavigableEdgeType.Space => 50,
                NavigableEdgeType.Jump => 500000000,
                NavigableEdgeType.Ground => 1,
                _ => (float)0,
            };
        }

        public float GetMilitaryPower()
        {
            return Composition.Sum(x => x.GetMilitaryPower());
        }

        public bool IsDestroyed()
        {
            return _isDestructable && Health.IsEmpty();
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetPosition(INavigable position)
        {
            var temp = Position;
            Position = position;
            position.Enter(Faction);
            Moved?.Invoke(this, MovementEventArgs.Create(temp, position));
        }
    }
}