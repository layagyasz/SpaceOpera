namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class DefendAction : IAction
    {
        public ActionType Type => ActionType.Combat;

        public static IAction Create()
        {
            return new DefendAction();
        }

        public bool Equivalent(IAction action)
        {
            return action is DefendAction;
        }

        public ActionStatus Progress(AtomicFormationDriver driver, World world)
        {
            world.Battles.Defend(driver.AtomicFormation);
            return ActionStatus.Done;
        }
    }
}
