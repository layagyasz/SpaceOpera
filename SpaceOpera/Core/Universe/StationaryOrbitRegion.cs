using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Universe
{
    public class StationaryOrbitRegion : INavigable
    {
        public string Name => string.Format("Geostationary {0} Orbit {1}", _parent?.Name, _identifier);
        public NavigableNodeType NavigableNodeType => NavigableNodeType.Space;
        public float Theta { get; }
        public List<StellarBodySubRegion> SubRegions { get; }

        private readonly string _identifier;
        private StellarBody? _parent;

        public StationaryOrbitRegion(string indentifier, float theta, IEnumerable<StellarBodySubRegion> subRegions)
        {
            _identifier = indentifier;
            Theta = theta;
            SubRegions = subRegions.ToList();
        }

        public void SetParent(StellarBody parent)
        {
            _parent = parent;
        }

        public void Enter(Faction faction) { }
    }
}