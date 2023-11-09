using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Universe
{
    public class StellarBody
    {
        public EventHandler<EventArgs>? OccupationChanged { get; set; }

        public string Name { get; private set; } = string.Empty;
        public string Type { get; }
        public Dictionary<string, object> Parameters { get; }
        public float Radius { get; }
        public float Mass { get; }
        public Orbit Orbit { get; }
        public Atmosphere Atmosphere { get; }
        public List<StellarBodyRegion> Regions { get; }
        public List<StationaryOrbitRegion> OrbitRegions { get; }
        
        public StellarBody(
            string type,
            Dictionary<string, object> parameters,
            float radius,
            float mass,
            Orbit orbit,
            Atmosphere atmosphere,
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

            foreach (var orbitRegion in OrbitRegions)
            {
                orbitRegion.SetParent(this);
            }
            foreach (var region in regions)
            {
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
            return .001f 
                * MathF.Pow(7464960000 * Constants.GravitationalConstant * Mass / (4 * MathF.PI * MathF.PI), 0.3333f)
                    - Radius;
        }

        public float GetHighOrbitAltitude()
        {
            return 4 * GetGeosynchronousOrbitAltitude();
        }

        public int GetSize()
        {
            return OrbitRegions.Count + Regions.Sum(x => x.SubRegions.Count);
        }

        public void ChangeOccupation()
        {
            OccupationChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }
}