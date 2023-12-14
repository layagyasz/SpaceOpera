namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class IdleAction : IAction
    {
        public ActionType Type => ActionType.None;

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

        public ActionStatus Progress(AtomicFormationDriver driver, World world)
        {
            if (Unassign)
            {
                driver.SetAssignment(Assigments.AssignmentType.None, overridePriority: true);
            }
            return ActionStatus.InProgress;
        }

        public override string ToString()
        {
            return "[IdleAction]";
        }
    }
}
