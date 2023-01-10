using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military.Actions
{
    class MoveAction : IAction
    {
        public ActionStatus Status { get; private set; } = ActionStatus.IN_PROGRESS;
        public NavigationMap.Movement Movement { get; }

        private double _Progress;

        public MoveAction(NavigationMap.Movement Movement)
        {
            this.Movement = Movement;
        }

        public bool Equivalent(IAction Action)
        {
            if (Action is MoveAction other)
            {
                return other.Movement.Destination == Movement.Destination;
            }
            return false;
        }

        public void Progress(IFormation Formation, World World)
        {
            _Progress += Formation.GetSpeed(Movement.Type);
            if (_Progress >= Movement.Distance)
            {
                Formation.SetPosition(Movement.Destination);
                Status = ActionStatus.DONE;
            }
            Status = ActionStatus.IN_PROGRESS;
        }

        public override string ToString()
        {
            return string.Format(
                "[MoveAction: Origin={0}, Destination={1}, Progress={2}/{3}",
                Movement.Origin.Name,
                Movement.Destination.Name,
                _Progress,
                Movement.Distance);
        }
    }
}