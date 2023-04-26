using Cardamom.Collections;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Core.Languages;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Orders;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Controller.Subcontrollers
{
    public class FleetSubcontroller : ISubcontroller
    {
        public EventHandler<IOrder>? OrderCreated { get; set; }

        private readonly HashSet<FormationDriver> _drivers;

        public FleetSubcontroller(IEnumerable<FormationDriver> drivers) 
        {
            _drivers = new(drivers);
        }

        public bool HandleInteraction(UiInteractionEventArgs interaction)
        {
            var obj = interaction.GetOnlyObject();
            if (obj != null && interaction.Button == MouseButton.Right)
            {
                if (obj is StarSystem || obj is INavigable)
                {
                    var nodes = GetNodes(obj);
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
                        OrderCreated?.Invoke(this, new SetFleetActiveRegionOrder(driver, activeNodes));
                    }
                }
                return true;
            }
            return false;
        }

        private static HashSet<INavigable> GetNodes(object @object)
        {
            if (@object is StarSystem system)
            {
                return NavigationMap.GetSystemNodes(system, new EnumSet<NavigableNodeType>(NavigableNodeType.Space));
            }
            else if (@object is LocalOrbitRegion localOrbit)
            {
                return NavigationMap.GetLocalOrbitNodes(
                    localOrbit, new EnumSet<NavigableNodeType>(NavigableNodeType.Space));
            }
            else if (@object is INavigable node)
            {
                if (node.NavigableNodeType == NavigableNodeType.Space)
                {
                    return new() { node };
                }
                else
                {
                    return new();
                }
            }
            throw new ArgumentException($"Unsupported interaction with {@object.GetType()}");
        }
    }
}
