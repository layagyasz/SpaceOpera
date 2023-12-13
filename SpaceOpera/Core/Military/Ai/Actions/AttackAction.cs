using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class AttackAction : IAction
    {
        public ActionType Type => ActionType.Combat;
        public INavigable Location { get; }

        private AttackAction(INavigable location)
        {
            Location = location;
        }

        public static IAction Create(INavigable location)
        {
            return new AttackAction(location);
        }

        public bool Equivalent(IAction action)
        {
            if (action is AttackAction other)
            {
                return other.Location == Location;
            }
            return false;
        }

        public ActionStatus Progress(AtomicFormationDriver driver, World world)
        {
            world.Battles.Attack(driver.AtomicFormation, Location);
            return ActionStatus.Done;
        }
    }
}
