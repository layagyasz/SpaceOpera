using OpenTK.Mathematics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.Scenes.Highlights
{
    public class FleetHighlight : IHighlight
    {
        public EventHandler<EventArgs>? OnUpdated { get; set; }

        public FleetDriver Fleet { get; }
        public Color4 BorderColor => Color4.White;
        public Color4 Color => new(0, 0, 0, 0);

        public FleetHighlight(FleetDriver fleet)
        {
            Fleet = fleet;
            fleet.OnOrderUpdated += HandleFleetUpdate;
        }

        public bool Contains(object @object)
        {
            if (@object is StarSystem system)
            {
                return Contains(system);
            }
            else if (@object is INavigable node)
            {
                return Contains(node);
            }
            return false;
        }

        public bool Contains(StarSystem starSystem)
        {
            return starSystem.Transits.Values.Any(Contains) 
                || starSystem.OrbitalRegions.Any(Contains)
                || starSystem.OrbitalRegions.Select(x => x.LocalOrbit).Any(Contains)
                || starSystem.OrbitalRegions.SelectMany(x => x.LocalOrbit.StellarBody.OrbitRegions).Any(Contains);
        }

        public bool Contains(INavigable navigable)
        {
            return Fleet.GetActiveRegion().Contains(navigable);
        }

        public void Unhook()
        {
            Fleet.OnOrderUpdated -= HandleFleetUpdate;
        }

        private void HandleFleetUpdate(object? sender, EventArgs e)
        {
            OnUpdated?.Invoke(this, e);
        }
    }
}