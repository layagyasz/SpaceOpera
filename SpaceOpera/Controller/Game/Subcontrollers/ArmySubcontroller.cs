using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Orders;
using SpaceOpera.Core.Orders.Formations.Assignments;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Controller.Game.Subcontrollers
{
    public class ArmySubcontroller : ISubcontroller
    {
        public EventHandler<IOrder>? OrderCreated { get; set; }

        private readonly HashSet<ArmyDriver> _drivers;

        public ArmySubcontroller(IEnumerable<ArmyDriver> drivers)
        {
            _drivers = new(drivers);
        }

        public bool HandleInteraction(UiInteractionEventArgs interaction)
        {
            var obj = interaction.GetOnlyObject();
            if (obj != null && interaction.Button == MouseButton.Right)
            {
                if (obj is StellarBodySubRegion region && region.ParentRegion!.DominantBiome.IsTraversable)
                {
                    var nodes = region.ParentRegion!.SubRegions;
                    foreach (var driver in _drivers)
                    {
                        var activeNodes = new HashSet<INavigable>(driver.GetActiveRegion());
                        if (nodes.All(activeNodes.Contains))
                        {
                            activeNodes.ExceptWith(nodes);
                        }
                        else
                        {
                            activeNodes.UnionWith(nodes);
                        }
                        OrderCreated?.Invoke(this, new SetActiveRegionOrder(driver, activeNodes));
                    }
                }
                return true;
            }
            return false;
        }
    }
}
