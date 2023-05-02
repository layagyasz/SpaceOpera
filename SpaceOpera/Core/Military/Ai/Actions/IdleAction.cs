namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class IdleAction : IAction
    {
        public ActionStatus Status { get; private set; } = ActionStatus.InProgress;

        public bool Unassign { get; }

        public IdleAction(bool unassign)
        {
            Unassign = unassign;
        }

        public bool Equivalent(IAction action)
        {
            if (action is IdleAction idle)
            {
                return Unassign == idle.Unassign;
            }
            return false;
        }

        public void Progress(IFormation formation, World world)
        {
            Status = ActionStatus.InProgress; 
        }
    }
}
