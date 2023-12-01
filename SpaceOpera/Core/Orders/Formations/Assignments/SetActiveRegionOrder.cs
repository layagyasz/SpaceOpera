using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Orders.Formations.Assignments
{
    public class SetActiveRegionOrder : IOrder
    {
        public IFormationDriver Driver { get; }
        public ISet<INavigable> ActiveRegion { get; }

        public SetActiveRegionOrder(IFormationDriver driver, ISet<INavigable> activeRegion)
        {
            Driver = driver;
            ActiveRegion = activeRegion;
        }

        public ValidationFailureReason Validate(World world)
        {
            return ValidationFailureReason.None;
        }

        public bool Execute(World world)
        {
            Driver.SetActiveRegion(ActiveRegion);
            return true;
        }
    }
}