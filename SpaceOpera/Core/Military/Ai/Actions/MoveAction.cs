using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class MoveAction : IAction
    {
        public ActionType Type => ActionType.Move;
        public NavigationMap.Movement Movement { get; }

        private double _progress;

        private MoveAction(NavigationMap.Movement movement)
        {
            Movement = movement;
        }

        public static MoveAction Create(NavigationMap.Movement movement)
        {
            return new(movement);
        }

        public bool Equivalent(IAction action)
        {
            if (action is MoveAction other)
            {
                return other.Movement.Destination == Movement.Destination;
            }
            return false;
        }

        public ActionStatus Progress(AtomicFormationDriver driver, World world)
        {
            var formation = driver.AtomicFormation;
            _progress += formation.GetSpeed(Movement.Type);
            if (_progress >= Movement.Distance)
            {
                formation.SetPosition(Movement.Destination);
                return ActionStatus.Done;
            }
            return ActionStatus.InProgress;
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