using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Orders;
using SpaceOpera.Core.Orders.Formations;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Controller.Game.Subcontrollers
{
    public class DivisionSubcontroller : ISubcontroller
    {
        public EventHandler<IOrder>? OrderCreated { get; set; }

        private readonly HashSet<DivisionDriver> _drivers;

        public DivisionSubcontroller(IEnumerable<DivisionDriver> drivers)
        {
            _drivers = new(drivers);
        }

        public bool HandleInteraction(UiInteractionEventArgs interaction)
        {
            var obj = interaction.GetOnlyObject();
            if (obj != null && interaction.Button == MouseButton.Right)
            {
                if (obj is StellarBodySubRegion region)
                {
                    if (region.Biome.IsTraversable)
                    {
                        foreach (var driver in _drivers)
                        {
                            OrderCreated?.Invoke(this, new MoveOrder(driver, region));
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}
