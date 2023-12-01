using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Orders.Formations
{
    public class JoinArmyOrder : IOrder
    {
        public ArmyDriver Army { get; }
        public AtomicFormationDriver Driver { get; }

        public JoinArmyOrder(ArmyDriver army, AtomicFormationDriver driver)
        {
            Army = army;
            Driver = driver;
        }

        public ValidationFailureReason Validate(World world)
        {
            return Driver.Parent == null ? ValidationFailureReason.None : ValidationFailureReason.IllegalOrder;
        }

        public bool Execute(World world)
        {
            return Army.Add(Driver);
        }
    }
}
