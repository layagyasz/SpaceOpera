namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class SpotAction : IAction
    {
        public ActionType Type => ActionType.Spot;
        public ActionStatus Status { get; private set; } = ActionStatus.InProgress;

        public IAtomicFormation Target { get; }

        public SpotAction(IAtomicFormation target)
        {
            Target = target;
        }

        public static IAction Create(IAtomicFormation target)
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

        public void Progress(IAtomicFormation formation, World world)
        {
            world.GetIntelligenceFor(formation.Faction).FleetIntelligence.Spot(Target, .5);
        }

        public override string ToString()
        {
            return "[SpotAction]";
        }
    }
}