namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class SpotAction : IAction
    {
        public ActionType Type => ActionType.Spot;

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

        public ActionStatus Progress(AtomicFormationDriver driver, World world)
        {
            world.GetIntelligenceFor(driver.AtomicFormation.Faction).FleetIntelligence.Spot(Target, .5);
            return ActionStatus.InProgress;
        }

        public override string ToString()
        {
            return "[SpotAction]";
        }
    }
}