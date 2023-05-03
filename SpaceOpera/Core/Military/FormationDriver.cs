using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Military.Ai.Assigments;
using SpaceOpera.Core.Universe;
using System.ComponentModel.Design.Serialization;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military
{
    public abstract class FormationDriver
    {
        public EventHandler<MovementEventArgs>? Moved { get; set; }
        public EventHandler<EventArgs>? OrderUpdated { get; set; }

        public IFormation Formation { get; }

        private readonly IFormationAi _ai;
        private IAction? _action;

        protected FormationDriver(IFormation formation, IFormationAi ai)
        {
            Formation = formation;
            Formation.Moved += HandleMove;

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
            Formation.Cohere();
            var newAction = _ai.Execute(context.ForFormation(Formation)).Result;
            if (newAction == null || _action == null || !newAction.Equivalent(_action))
            {
                _action = newAction;
            }
            _action?.Progress(Formation, context.World);
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