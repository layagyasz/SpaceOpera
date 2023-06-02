using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class UnloadAction : IAction
    {
        public ActionType Type => ActionType.Unload;

        public EconomicZone EconomicZone { get; }

        public UnloadAction(EconomicZone economicZone)
        {
            EconomicZone = economicZone;
        }

        public bool Equivalent(IAction other)
        {
            return false;
        }

        public ActionStatus Progress(AtomicFormationDriver driver, World world)
        {
            return ActionStatusMapper.ToActionStatus(EconomicZone.Unload(driver.AtomicFormation.Inventory));
        }
    }
}
