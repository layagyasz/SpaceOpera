namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class RetreatAction : IAction
    {
        public ActionType Type => ActionType.Move;

        public static IAction Create()
        {
            return new RetreatAction();
        }

        public bool Equivalent(IAction action)
        {
            return action is RetreatAction;
        }

        public ActionStatus Progress(AtomicFormationDriver driver, World world)
        {
            world.Battles.Withdraw(driver.AtomicFormation);
            driver.SetAssignment(Assigments.AssignmentType.Retreat);
            return ActionStatus.Done;
        }
    }
}
