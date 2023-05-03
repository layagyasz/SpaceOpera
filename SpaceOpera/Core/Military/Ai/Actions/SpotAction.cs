namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class SpotAction : IAction
    {
        public ActionType Type => ActionType.Spot;
        public ActionStatus Status { get; private set; } = ActionStatus.InProgress;

        public IFormation Target { get; }

        public SpotAction(IFormation target)
        {
            Target = target;
        }

        public static IAction Create(IFormation target)
        {
            return new SpotAction(target);
        }

        public bool Equivalent(IAction action)
        {
            if (action is SpotAction other)
            {
                return other.Target == Target;
            }
            return false;
        }

        public void Progress(IFormation formation, World world)
        {
            world.GetIntelligenceFor(formation.Faction).FleetIntelligence.Spot(Target, .5);
        }

        public override string ToString()
        {
            return "[SpotAction]";
        }
    }
}