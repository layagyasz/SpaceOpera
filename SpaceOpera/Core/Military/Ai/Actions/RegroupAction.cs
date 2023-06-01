namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class RegroupAction : IAction
    {
        public ActionType Type => ActionType.Regroup;

        public bool Equivalent(IAction action)
        {
            if (action is RegroupAction)
            {
                return true;
            }
            return false;
        }

        public ActionStatus Progress(AtomicFormationDriver driver, World world)
        {
            return driver.AtomicFormation.Cohesion.IsFull() ? ActionStatus.Done : ActionStatus.InProgress;
        }

        public override string ToString()
        {
            return "[RegroupAction]";
        }
    }
}

