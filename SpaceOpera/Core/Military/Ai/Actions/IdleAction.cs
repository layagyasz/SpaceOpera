namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class IdleAction : IAction
    {
        public ActionStatus Status { get; private set; } = ActionStatus.InProgress;

        public bool Equivalent(IAction action)
        {
            if (action is IdleAction)
            {
                return true;
            }
            return false;
        }

        public void Progress(IFormation formation, World world)
        {
            Status = ActionStatus.InProgress; 
        }
    }
}
