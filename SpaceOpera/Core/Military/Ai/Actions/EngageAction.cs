namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class EngageAction : IAction
    {
        public ActionStatus Status { get; private set; } = ActionStatus.InProgress;
        public IFormation Target { get; }

        private EngageAction(IFormation target)
        {
            Target = target;
        }

        public static IAction Create(IFormation target)
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
