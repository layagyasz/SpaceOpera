using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Orders.Formations
{
    public class LeaveArmyOrder : IOrder
    {
        public DivisionDriver Driver { get; }

        public LeaveArmyOrder(DivisionDriver driver)
        {
            Driver = driver;
        }

        public ValidationFailureReason Validate(World world)
        {
            return Driver.Parent == null ? ValidationFailureReason.IllegalOrder : ValidationFailureReason.None;
        }

        public bool Execute(World world)
        {
            var parent = (ArmyDriver)Driver.Parent!;
            return parent.Remove(Driver);
        }
    }
}
