using Cardamom.Trackers;
using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class LoadAction : IAction
    {
        public ActionType Type => ActionType.Load;

        public EconomicZone EconomicZone { get; }
        public MultiQuantity<IMaterial> Materials { get; }

        public LoadAction(EconomicZone economicZone, MultiQuantity<IMaterial> materials)
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
            return ActionStatusMapper.ToActionStatus(EconomicZone.Load(driver.AtomicFormation.Inventory, Materials));
        }
    }
}
