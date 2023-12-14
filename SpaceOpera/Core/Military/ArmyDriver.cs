using SpaceOpera.Core.Military.Ai;
using SpaceOpera.Core.Military.Ai.Assigments;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military
{
    public class ArmyDriver : IFormationDriver
    {
        public EventHandler<EventArgs>? OrderUpdated { get; set; }
        public IFormation Formation => Army;
        public Army Army { get; }

        private readonly List<AtomicFormationDriver> _divisions = new();

        private IAssigner _assigner;

        public ArmyDriver(Army army)
        {
            Army = army;
            _assigner = new NoAssigner();
        }

        public bool Add(AtomicFormationDriver driver)
        {
            if (driver.Formation is Division division && driver.Parent == null)
            {
                _divisions.Add(driver);
                Army.Add(division);
                driver.Parent = this;
                return true;
            }
            return false;
        }

        public AssignmentType GetAssignment()
        {
            return _assigner.Type;
        }

        public IEnumerable<AtomicFormationDriver> GetDivisions()
        {
            return _divisions;
        }

        public bool Remove(AtomicFormationDriver division)
        {
            if (_divisions.Remove(division))
            {
                Army.Remove((Division)division.Formation);
                division.Parent = null;
                return true;
            }
            return false;
        }

        public void SetAssignment(AssignmentType type, bool overridePriority = false)
        {
            if (type == _assigner.Type)
            {
                return;
            }
            switch (type)
            {
                case AssignmentType.Defend:
                    SetAssigner(new DefendAssigner());
                    break;
                case AssignmentType.None:
                    SetAssigner(new NoAssigner());
                    break;
                case AssignmentType.Train:
                    SetAssigner(new TrainAssigner());
                    break;
            }
            OrderUpdated?.Invoke(this, EventArgs.Empty);
        }

        public ICollection<INavigable> GetActiveRegion()
        {
            return _assigner.GetActiveRegion();
        }

        public void SetActiveRegion(IEnumerable<INavigable> activeRegion)
        {
            _assigner.SetActiveRegion(activeRegion);
            OrderUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void Tick(SpaceOperaContext context)
        {
            lock (_divisions)
            {
                _divisions.RemoveAll(x => x.Formation.IsDestroyed());
            }
            Army.CheckDivisions();
            _assigner.Tick(_divisions, context);
        }

        private void SetAssigner(IAssigner assigner)
        {
            assigner.SetActiveRegion(_assigner.GetActiveRegion());
            _assigner = assigner;
        }
    }
}
