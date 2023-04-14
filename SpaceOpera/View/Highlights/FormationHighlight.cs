using OpenTK.Mathematics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.Highlights
{
    public class FormationHighlight : IHighlight
    {
        public bool Dirty { get; set; }
        public bool Merge => true;
        public float BorderWidth => 4f;
        public Color4 BorderColor => Color4.Yellow;
        public Color4 Color => new(0, 0, 0, 0);

        private readonly HashSet<IFormationDriver> _drivers;

        public FormationHighlight(IEnumerable<IFormationDriver> drivers)
        {
            _drivers = new(drivers);
            foreach (var driver in _drivers)
            {
                driver.OrderUpdated += HandleFleetUpdate;
            }
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
            return _drivers.Any(x => x.GetActiveRegion().Contains(navigable));
        }

        public void Unhook()
        {
            foreach (var driver in _drivers)
            {
                driver.OrderUpdated -= HandleFleetUpdate;
            }
        }

        private void HandleFleetUpdate(object? sender, EventArgs e)
        {
            Dirty = true;
        }
    }
}