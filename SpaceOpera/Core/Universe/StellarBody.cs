using Cardamom.Mathematics.Coordinates;
using OpenTK.Mathematics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Universe
{
    public class StellarBody
    {
        public EventHandler<ValueEventArgs<Division>>? OnDivisionAdded { get; set; }

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

        public float GetHighOrbitAltitude()
        {
            return 4 * GetGeosynchronousOrbitAltitude();
        }

        public double GetSolarOrbitDistance(double angle)
        {
            return Orbit.GetDistance(angle);
        }

        public Polar2 GetSolarOrbitPosition(double angle)
        {
            return new((float)GetSolarOrbitDistance(angle), (float)angle);
        }

        public double GetSolarOrbitProgression(double angle, float precision, int accuracy)
        {
            double e = angle;
            double f = e - Orbit.Eccentricity * Math.Sin(angle) - angle;

            int i = 0;
            while ((Math.Abs(f) > precision) && (i < accuracy))
            {
                e = e - f / (1f - Orbit.Eccentricity * Math.Cos(e));
                f = e - Orbit.Eccentricity * Math.Sin(e) - angle;
                ++i;
            }

            double sin = Math.Sin(e);
            double cos = Math.Cos(e);
            double fak = Math.Sqrt(1f - Orbit.Eccentricity * Orbit.Eccentricity);
            return Math.Atan2(fak * sin, cos - Orbit.Eccentricity);
        }

        public float GetYearLengthInMillis()
        {
            return 2 * MathHelper.Pi * MathF.Sqrt(
                MathF.Pow(1000 * Orbit.MajorAxis / (2 * Constants.AstralUnit), 3)
                / (Constants.GravitationalConstant * Orbit.Focus.Mass));
        }

        public int GetSize()
        {
            return OrbitRegions.Count + Regions.Sum(x => x.SubRegions.Count);
        }

        public void SetName(string name)
        {
            Name = name;
        }

        private void HandleDivisionAdded(object? sender, ValueEventArgs<Division> e)
        {
            OnDivisionAdded?.Invoke(this, e);
        }
    }
}