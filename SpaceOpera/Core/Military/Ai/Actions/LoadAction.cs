using Cardamom.Trackers;
using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class LoadAction : IAction
    {
        public ActionType Type => ActionType.Load;

        public EconomicZoneHolding EconomicZone { get; }
        public MultiQuantity<IMaterial> Materials { get; }

        public LoadAction(EconomicZoneHolding economicZone, MultiQuantity<IMaterial> materials)
        {
            EconomicZone = economicZone;
            Materials = materials;
        }

        public bool Equivalent(IAction other)
        {
            return false;
        }

        public ActionStatus Progress(AtomicFormationDriver driver, World world)
        {
            return ActionStatusMapper.ToActionStatus(
                driver.AtomicFormation.Inventory.MaxTransferFrom(
                    EconomicZone.GetInventory(), Materials, float.MaxValue));
        }
    }
}
