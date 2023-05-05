using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Ai.Assigments;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Orders.Formations
{
    public class MoveOrder : IOrder
    {
        public AtomicFormationDriver Driver { get; }
        public INavigable Destination { get; }

        public MoveOrder(AtomicFormationDriver driver, INavigable destination)
        {
            Driver = driver;
            Destination = destination;
        }

        public ValidationFailureReason Validate()
        {
            if (Driver is DivisionDriver)
            {
                return Destination.NavigableNodeType == NavigableNodeType.Ground
                    ?  ValidationFailureReason.None : ValidationFailureReason.IllegalOrder;
            }
            return ValidationFailureReason.IllegalOrder;
        }

        public bool Execute(World world)
        {
            Driver.SetAssignment(AssignmentType.Move);
            Driver.SetActiveRegion(new List<INavigable>() { Destination });
            return true;
        }
    }
}