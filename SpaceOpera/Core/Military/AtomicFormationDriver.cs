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

        private readonly IFormationAi _ai;
        private IAction? _action;

        protected AtomicFormationDriver(IAtomicFormation formation, IFormationAi ai)
        {
            AtomicFormation = formation;
            AtomicFormation.Moved += HandleMove;

            _ai = ai;
        }

        public IAction? GetCurrentAction()
        {
            return _action;
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
            var newAction = _ai.Execute(context.ForFormation(AtomicFormation)).Result;
            if (newAction == null || _action == null || !newAction.Equivalent(_action))
            {
                _action = newAction;
            }
            _action?.Progress(AtomicFormation, context.World);
            if (_action is IdleAction idle)
            {
                if (idle.Unassign)
                {
                    SetAssignment(AssignmentType.None);
                }
            }
        }

        private void HandleMove(object? sender, MovementEventArgs e)
        {
            Moved?.Invoke(this, e);
        }
    }
}