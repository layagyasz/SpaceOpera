namespace SpaceOpera.Core.Military.Actions
{
    public class EngageAction : IAction
    {
        public ActionStatus Status { get; private set; } = ActionStatus.InProgress;
        public Fleet Target { get; }

        private EngageAction(Fleet target)
        {
            Target = target;
        }

        public static EngageAction Create(Fleet target)
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

        public void Progress(IFormation formation, World world)
        {
            world.BattleManager.Engage(formation, Target);
        }
    }
}
