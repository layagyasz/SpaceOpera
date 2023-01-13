namespace SpaceOpera.Core.Military.Actions
{
    public class SpotAction : IAction
    {
        public ActionStatus Status { get; private set; } = ActionStatus.InProgress;

        public Fleet Target { get; }

        public SpotAction(Fleet target)
        {
            Target = target;
        }

        public static IAction Create(Fleet target)
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