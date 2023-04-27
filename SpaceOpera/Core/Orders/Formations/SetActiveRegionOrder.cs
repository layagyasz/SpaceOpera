using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Orders.Formations
{
    public class SetActiveRegionOrder : IOrder
    {
        public FormationDriver Driver { get; }
        public ISet<INavigable> ActiveRegion { get; }

        public SetActiveRegionOrder(FormationDriver driver, ISet<INavigable> activeRegion)
        {
            Driver = driver;
            ActiveRegion = activeRegion;
        }

        public ValidationFailureReason Validate()
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