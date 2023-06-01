namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class EngageAction : IAction
    {
        public ActionType Type => ActionType.Combat;
        public IAtomicFormation Target { get; }

        private EngageAction(IAtomicFormation target)
        {
            Target = target;
        }

        public static IAction Create(IAtomicFormation target)
        {
            return new EngageAction(target);
        }

        public bool Equivalent(IAction action)
        {
            if (action is EngageAction other)
            {
                return other.Target == Target;
            }
            return false;
        }

        public ActionStatus Progress(AtomicFormationDriver driver, World world)
        {
            world.BattleManager.Engage(driver.AtomicFormation, Target);
            return ActionStatus.Done;
        }
    }
}
