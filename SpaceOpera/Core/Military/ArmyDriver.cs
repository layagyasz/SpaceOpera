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

        private IAssigner _assigner;

        public ArmyDriver(Army army)
        {
            Army = army;
            _assigner = new NoAssigner();
        }

        public AssignmentType GetAssignment()
        {
            return _assigner.Type;
        }

        public void SetAssignment(AssignmentType type)
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
            _assigner.Tick(context);
        }

        private void SetAssigner(IAssigner assigner)
        {
            assigner.SetActiveRegion(_assigner.GetActiveRegion());
            _assigner = assigner;
        }
    }
}
