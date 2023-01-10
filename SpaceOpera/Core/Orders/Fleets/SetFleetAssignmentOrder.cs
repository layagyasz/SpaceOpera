using SpaceOpera.Core.Military;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Orders.Fleets
{
    class SetFleetAssignmentOrder : IImmediateOrder
    {
        public FleetDriver Fleet { get; }
        public FleetAssignment Action { get; }

        public SetFleetAssignmentOrder(FleetDriver Fleet, FleetAssignment Action)
        {
            this.Fleet = Fleet;
            this.Action = Action;
        }

        public ValidationFailureReason Validate()
        {
            return ValidationFailureReason.NONE;
        }

        public bool Execute(World World)
        {
            Fleet.SetAssignment(Action);
            return true;
        }
    }
}