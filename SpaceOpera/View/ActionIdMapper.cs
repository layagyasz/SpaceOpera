using SpaceOpera.Core.Military.Ai.Assigments;
using SpaceOpera.View.Panes;

namespace SpaceOpera.View
{
    public static class ActionIdMapper
    {
        public static ActionId ToActionId(AssignmentType assignmentType)
        {
            return assignmentType switch
            {
                AssignmentType.Defend => ActionId.Defend,
                AssignmentType.Logistics => ActionId.Logistics,
                AssignmentType.Move => ActionId.Move,
                AssignmentType.None => ActionId.NoAssignment,
                AssignmentType.Patrol => ActionId.Patrol,
                AssignmentType.Train => ActionId.Train,
                _ => ActionId.Unknown
            };
        }

        public static AssignmentType ToAssignmentType(ActionId id)
        {
            return id switch
            {
                ActionId.Defend => AssignmentType.Defend,
                ActionId.Logistics => AssignmentType.Logistics,
                ActionId.Move => AssignmentType.Move,
                ActionId.NoAssignment => AssignmentType.None,
                ActionId.Patrol => AssignmentType.Patrol,
                ActionId.Train => AssignmentType.Train,
                _ => AssignmentType.Unknown,
            };
        }

        public static int? ToGameSpeed(ActionId id)
        {
            return id switch
            {
                ActionId.GameSpeedPause => 0,
                ActionId.GameSpeedNormal => 1,
                ActionId.GameSpeedFast => 8,
                _ => null,
            };
        }

        public static GamePaneId ToPaneId(ActionId id)
        {
            return id switch
            {
                ActionId.Equipment => GamePaneId.Equipment,
                ActionId.Logistics => GamePaneId.Logistics,
                ActionId.Military => GamePaneId.Military,
                ActionId.MilitaryOrganization => GamePaneId.MilitaryOrganization,
                ActionId.Research => GamePaneId.Research,
                ActionId.Trade => GamePaneId.Trade,
                _ => GamePaneId.Unknown,
            };
        }
    }
}
