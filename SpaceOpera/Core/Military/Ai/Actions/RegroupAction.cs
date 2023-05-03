namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class RegroupAction : IAction
    {
        public ActionType Type => ActionType.Regroup;
        public ActionStatus Status { get; private set; } = ActionStatus.InProgress;

        public bool Equivalent(IAction action)
        {
            if (action is RegroupAction)
            {
                return true;
            }
            return false;
        }

        public void Progress(IFormation formation, World world)
        {
            if (formation.Cohesion.IsFull())
            {
                Status = ActionStatus.Done;
            }
        }

        public override string ToString()
        {
            return "[RegroupAction]";
        }
    }
}

