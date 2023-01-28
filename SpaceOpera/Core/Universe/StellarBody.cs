using Cardamom.Trackers;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Universe
{
    public class StellarBody
    {
        public EventHandler<ElementEventArgs<Division>>? OnDivisionAdded { get; set; }

        public string Name { get; private set; } = string.Empty;
        public string Type { get; }
        public Dictionary<string, object> Parameters { get; }
        public float Radius { get; }
        public float Mass { get; }
        public Orbit Orbit { get; }
        public MultiQuantity<IMaterial> Atmosphere { get; }
        public List<StellarBodyRegion> Regions { get; }
        public List<StationaryOrbitRegion> OrbitRegions { get; }
        
        public StellarBody(
            string type,
            Dictionary<string, object> parameters,
            float radius,
            float mass,
            Orbit orbit,
            MultiQuantity<IMaterial> atmosphere,
            IEnumerable<StellarBodyRegion> regions, 
            IEnumerable<StationaryOrbitRegion> orbitRegions)
        {
            Type = type;
            Parameters = parameters;
            Radius = radius;
            Mass = mass;
            Orbit = orbit;
            Atmosphere = atmosphere;
            Regions = regions.ToList();
            OrbitRegions = orbitRegions.ToList();

            foreach (var region in regions)
            {
                region.OnDivisionAdded += HandleDivisionAdded;
                region.SetParent(this);
            }
        }

        public bool ContainsFaction(Faction faction)
        {
            return Regions.Any(x => x.Sovereign == faction);
        }

        public float GetGeosynchronousOrbitAltitude()
        {
            // 24hrs.  Use random day length instead.
            return Constants.AstralUnit 
                * (MathF.Pow(7464960000 * Constants.GravitationalConstant * Mass / (4 * MathF.PI * MathF.PI), 0.3333f) 
                    - Radius);
        }

        public double GetHighOrbitAltitude()
        {
            return 4 * GetGeosynchronousOrbitAltitude();
        }

        public void SetName(string name)
        {
            Name = name;
        }

        private void HandleDivisionAdded(object? sender, ElementEventArgs<Division> e)
        {
            OnDivisionAdded?.Invoke(this, e);
        }
    }
}