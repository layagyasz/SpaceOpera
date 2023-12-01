using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military.Ai;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Military.Ai.Assigments;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military
{
    public abstract class AtomicFormationDriver : IFormationDriver
    {
        public EventHandler<MovementEventArgs>? Moved { get; set; }
        public EventHandler<EventArgs>? OrderUpdated { get; set; }

        public IFormation Formation => AtomicFormation;
        public IAtomicFormation AtomicFormation { get; }
        public IFormationDriver? Parent { get; set; }

        private readonly IFormationAi _ai;
        private IAction _action;
        private ActionStatus _actionStatus;

        protected AtomicFormationDriver(IAtomicFormation formation, IFormationAi ai)
        {
            AtomicFormation = formation;
            AtomicFormation.Moved += HandleMove;

            _ai = ai;
            _action = new IdleAction(false);
            _actionStatus = ActionStatus.InProgress;
        }

        public IAction GetCurrentAction()
        {
            return _action;
        }

        public ActionStatus GetCurrentActionStatus()
        {
            return _actionStatus;
        }

        public AssignmentType GetAssignment()
        {
            return _ai.GetAssignment().Type;
        }

        public void SetAssignment(AssignmentType type)
        {
            if (type == _ai.GetAssignment().Type)
            {
                return;
            }
            switch (type)
            {
                case AssignmentType.Move:
                    _ai.SetAssignment(new MoveAssignment());
                    break;
                case AssignmentType.None:
                    _ai.SetAssignment(new NoAssignment());
                    break;
                case AssignmentType.Patrol:
                    _ai.SetAssignment(new PatrolAssignment());
                    break;
                case AssignmentType.Train:
                    _ai.SetAssignment(new TrainAssignment());
                    break;
            }
            OrderUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void SetPersistentRoute(PersistentRoute route)
        {
            if (_ai.GetAssignment() is LogisticsAssignment current)
            {
                if (current.Route == route)
                {
                    return;
                }
            }
            _ai.SetAssignment(new LogisticsAssignment(route));
            OrderUpdated?.Invoke(this, EventArgs.Empty);
        }

        public ICollection<INavigable> GetActiveRegion()
        {
            return _ai.GetActiveRegion();
        }

        public void SetActiveRegion(IEnumerable<INavigable> activeRegion)
        {
            _ai.SetActiveRegion(activeRegion);
            OrderUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void Tick(SpaceOperaContext context)
        {
            AtomicFormation.Cohere();
            var newAction = _ai.Execute(context.ForDriver(this)).Result!;
            if (!newAction.Equivalent(_action))
            {
                _action = newAction;
            }
            _actionStatus = _action.Progress(this, context.World);
        }

        private void HandleMove(object? sender, MovementEventArgs e)
        {
            Moved?.Invoke(this, e);
        }
    }
}