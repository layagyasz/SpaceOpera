using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe
{
    public class StellarBody
    {
        public EventHandler<ElementEventArgs<Division>> OnDivisionAdded { get; set; }

        public string Name { get; private set; }
        public string Type { get; }
        public double Radius { get; }
        public double Mass { get; }
        public Orbit Orbit { get; }
        public MultiQuantity<IMaterial> Atmosphere { get; }
        public List<StellarBodyRegion> Regions { get; }
        public List<StationaryOrbitRegion> OrbitRegions { get; }
        
        public StellarBody(
            string Type,
            double Radius,
            double Mass,
            Orbit Orbit,
            MultiQuantity<IMaterial> Atmosphere,
            IEnumerable<StellarBodyRegion> Regions, 
            IEnumerable<StationaryOrbitRegion> OrbitRegions)
        {
            this.Name = Name;
            this.Type = Type;
            this.Radius = Radius;
            this.Mass = Mass;
            this.Orbit = Orbit;
            this.Atmosphere = Atmosphere;
            this.Regions = Regions.ToList();
            this.OrbitRegions = OrbitRegions.ToList();

            foreach (var region in Regions)
            {
                region.OnDivisionAdded += HandleDivisionAdded;
                region.SetParent(this);
            }
        }

        public bool ContainsFaction(Faction Faction)
        {
            return Regions.Any(x => x.Sovereign == Faction);
        }

        public double GetGeosynchronousOrbitAltitude()
        {
            // 24hrs.  Use random day length instead.
            return Constants.ASTRAL_UNIT 
                * (Math.Pow(7464960000 * Constants.GRAVITATIONAL_CONSTANT * Mass / (4 * Math.PI * Math.PI), 0.3333) 
                    - Radius);
        }

        public double GetHighOrbitAltitude()
        {
            return 4 * GetGeosynchronousOrbitAltitude();
        }

        public void SetName(string Name)
        {
            this.Name = Name;
        }

        private void HandleDivisionAdded(object Sender, ElementEventArgs<Division> E)
        {
            OnDivisionAdded?.Invoke(this, E);
        }
    }
}