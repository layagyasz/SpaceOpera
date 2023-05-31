using Cardamom.Collections;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Ai.Assigments;

namespace SpaceOpera.Core.Orders.Formations
{
    public class SetAssignmentOrder : IOrder
    {
        private static readonly EnumSet<AssignmentType> s_ArmyAssignments =
            new(AssignmentType.None, AssignmentType.Defend, AssignmentType.Train);
        private static readonly EnumSet<AssignmentType> s_FleetAssignments =
            new(AssignmentType.None, AssignmentType.Patrol);
        private static readonly EnumSet<AssignmentType> s_DivisionAssignments = 
            new(AssignmentType.None, AssignmentType.Train);

        public IFormationDriver Driver { get; }
        public AssignmentType Assignment { get; }

        public SetAssignmentOrder(IFormationDriver driver, AssignmentType assignment)
        {
            Driver = driver;
            Assignment = assignment;
        }

        public ValidationFailureReason Validate(World world)
        {
            if (Driver is ArmyDriver)
            {
                return s_ArmyAssignments.Contains(Assignment) 
                    ? ValidationFailureReason.None 
                    : ValidationFailureReason.IllegalOrder;
            }
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
