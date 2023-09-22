using OpenTK.Mathematics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.Game.Highlights
{
    public class FormationHighlight : RegionHighlight
    {
        public override EventHandler<EventArgs>? Updated { get; set; }

        public override bool Merge => true;
        public override float BorderWidth => 4f;
        public override Color4 BorderColor => Color4.Yellow;
        public override Color4 Color => Color4.Yellow;

        private readonly HashSet<IFormationDriver> _drivers;

        public FormationHighlight(IEnumerable<IFormationDriver> drivers)
        {
            _drivers = new(drivers);
        }

        public override bool Contains(object @object)
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
            return _drivers.Any(x => x.GetActiveRegion().Contains(navigable));
        }

        public override void Hook(object domain)
        {
            foreach (var driver in _drivers)
            {
                driver.OrderUpdated += HandleFleetUpdate;
            }
        }

        public override void Unhook(object domain)
        {
            foreach (var driver in _drivers)
            {
                driver.OrderUpdated -= HandleFleetUpdate;
            }
        }

        private void HandleFleetUpdate(object? sender, EventArgs e)
        {
            Updated?.Invoke(this, e);
        }
    }
}