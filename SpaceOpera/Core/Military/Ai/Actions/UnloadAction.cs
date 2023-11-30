using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class UnloadAction : IAction
    {
        public ActionType Type => ActionType.Unload;

        public EconomicZoneHolding EconomicZone { get; }

        public UnloadAction(EconomicZoneHolding economicZone)
        {
            EconomicZone = economicZone;
        }

        public bool Equivalent(IAction other)
        {
            return false;
        }

        public ActionStatus Progress(AtomicFormationDriver driver, World world)
        {
            return ActionStatusMapper.ToActionStatus(
                driver.AtomicFormation.Inventory.MaxTransferTo(EconomicZone.GetInventory(), float.MaxValue));
        }
    }
}
