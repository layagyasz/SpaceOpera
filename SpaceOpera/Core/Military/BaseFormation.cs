using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military
{
    abstract class BaseFormation : IFormation
    {
        private static readonly float BASE_RECOHERANCE = 0.1f;

        public EventHandler<MovementEventArgs> OnMoved { get; set; }

        public string Name { get; private set; }
        public Faction Faction { get; }
        public INavigable Position { get; private set; }
        public Pool Cohesion { get; } = new Pool(1);
        public List<UnitGrouping> Composition { get; } = new List<UnitGrouping>();
        public bool InCombat { get; private set; }

        protected BaseFormation(Faction Faction)
        {
            this.Faction = Faction;
        }

        public void Add(UnitGrouping UnitGrouping)
        {
            var currentGrouping = Composition.Where(x => x.Unit == UnitGrouping.Unit).FirstOrDefault();
            if (currentGrouping != null)
            {
                currentGrouping.Combine(UnitGrouping);
            }
            Composition.Add(UnitGrouping);
        }

        public void Add(Count<Unit> Unit)
        {
            Add(new UnitGrouping(Unit));
        }

        public void Add(MultiCount<Unit> Units)
        {
            foreach (var unit in Units.GetCounts())
            {
                Add(unit);
            }
        }

        public void Cohere()
        {
            Cohesion.ChangeAmount((InCombat ? .1f : 1) * BASE_RECOHERANCE);
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
            return Composition.Sum(x => x.Unit.Command * x.Count);
        }

        public float GetDetection()
        {
            return Composition.Max(x => x.Unit.Detection.UnitValue);
        }

        public float GetEvasion()
        {
            return Composition.Min(x => x.Unit.Evasion.UnitValue);
        }

        public float GetSpeed(NavigableEdgeType Type)
        {
            switch (Type)
            {
                case NavigableEdgeType.SPACE:
                    return 50;
                case NavigableEdgeType.JUMP:
                    return 500000000;
                case NavigableEdgeType.GROUND:
                    return 1;
                default:
                    return 0;
            }
        }

        public void SetName(string Name)
        {
            this.Name = Name;
        }

        public void SetPosition(INavigable Position)
        {
            var temp = this.Position;
            this.Position = Position;
            OnMoved?.Invoke(this, MovementEventArgs.Create(temp, Position));
        }
    }
}