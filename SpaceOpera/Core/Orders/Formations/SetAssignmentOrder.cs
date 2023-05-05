using Cardamom.Collections;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Ai.Assigments;

namespace SpaceOpera.Core.Orders.Formations
{
    public class SetAssignmentOrder : IOrder
    {
        private static readonly EnumSet<AssignmentType> s_FleetAssignments =
            new(AssignmentType.None, AssignmentType.Patrol);
        private static readonly EnumSet<AssignmentType> s_DivisionAssignments = 
            new(AssignmentType.None, AssignmentType.Train);

        public AtomicFormationDriver Driver { get; }
        public AssignmentType Assignment { get; }

        public SetAssignmentOrder(AtomicFormationDriver driver, AssignmentType assignment)
        {
            Driver = driver;
            Assignment = assignment;
        }

        public ValidationFailureReason Validate()
        {
            if (Driver is FleetDriver)
            {
                return s_FleetAssignments.Contains(Assignment) 
                    ? ValidationFailureReason.None : ValidationFailureReason.IllegalOrder;
            }
            if (Driver is DivisionDriver)
            {
                return s_DivisionAssignments.Contains(Assignment)
                    ? ValidationFailureReason.None : ValidationFailureReason.IllegalOrder;
            }
            return ValidationFailureReason.IllegalOrder;
        }

        public bool Execute(World world)
        {
            Driver.SetAssignment(Assignment);
            return true;
        }
    }
}
