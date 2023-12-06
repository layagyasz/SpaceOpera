using Cardamom.Utils;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Orders;
using SpaceOpera.Core.Orders.Formations;
using SpaceOpera.View;

namespace SpaceOpera.Controller.Game.Panes.MilitaryPanes
{
    public class ArmySummaryController : TabComponentController
    {
        private readonly ArmyDriver _driver;

        public ArmySummaryController(ArmyDriver driver)
        {
            _driver = driver;
        }

        protected override Optional<IOrder> InterceptInteraction(UiInteractionEventArgs e)
        {
            if (e.GetOnlyObject() is DivisionDriver division)
            {
                if (e.Action == ActionId.Split)
                {
                    return Optional<IOrder>.Of(new LeaveArmyOrder(division));
                }
                else if (e.Action == ActionId.Join)
                {
                    return Optional<IOrder>.Of(new JoinArmyOrder(_driver, division));
                }
            }
            return Optional<IOrder>.Empty();
        }
    }
}
