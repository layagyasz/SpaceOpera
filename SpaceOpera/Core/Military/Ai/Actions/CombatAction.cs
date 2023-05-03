namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class CombatAction : IAction
    {
        public ActionType Type => ActionType.Combat;
        public ActionStatus Status { get; private set; } = ActionStatus.InProgress;

        public bool Equivalent(IAction action)
        {
            if (action is CombatAction)
            {
                return true;
            }
            return false;
        }

        public void Progress(IFormation formation, World world)
        {
            if (!formation.InCombat)
            {
                Status = ActionStatus.Done;
            }
        }
    }
}
