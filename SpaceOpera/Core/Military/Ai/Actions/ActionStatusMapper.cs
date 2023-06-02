using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Military.Ai.Actions
{
    public static class ActionStatusMapper
    {
        public static ActionStatus ToActionStatus(Inventory.ChangeStatus changeStatus)
        {
            return changeStatus switch
            {
                Inventory.ChangeStatus.InProgress => ActionStatus.InProgress,
                Inventory.ChangeStatus.Blocked => ActionStatus.Blocked,
                Inventory.ChangeStatus.Done => ActionStatus.Done,
                _ => ActionStatus.Unknown,
            };
        }
    }
}
