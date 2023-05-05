using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class MoveAction : IAction
    {
        public ActionType Type => ActionType.Move;
        public ActionStatus Status { get; private set; } = ActionStatus.InProgress;
        public NavigationMap.Movement Movement { get; }

        private double _progress;

        public MoveAction(NavigationMap.Movement movement)
        {
            Movement = movement;
        }

        public bool Equivalent(IAction action)
        {
            if (action is MoveAction other)
            {
                return other.Movement.Destination == Movement.Destination;
            }
            return false;
        }

        public void Progress(IAtomicFormation formation, World world)
        {
            _progress += formation.GetSpeed(Movement.Type);
            if (_progress >= Movement.Distance)
            {
                formation.SetPosition(Movement.Destination);
                Status = ActionStatus.Done;
            }
            Status = ActionStatus.InProgress;
        }

        public override string ToString()
        {
            return string.Format(
                "[MoveAction: Origin={0}, Destination={1}, Progress={2}/{3}",
                Movement.Origin.Name,
                Movement.Destination.Name,
                _progress,
                Movement.Distance);
        }
    }
}