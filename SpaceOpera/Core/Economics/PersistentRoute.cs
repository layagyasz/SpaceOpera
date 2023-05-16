using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Economics
{
    public class PersistentRoute : ITickable
    {
        public Faction Faction { get; set; }
        public EconomicZone LeftZone { get; }
        public MultiQuantity<IMaterial> LeftMaterials { get; }

        public EconomicZone RightZone { get; set; }
        public MultiQuantity<IMaterial> RightMaterials { get; }

        public PersistentRoute(
            Faction faction,
            EconomicZone leftZone, 
            MultiQuantity<IMaterial> leftMaterials,
            EconomicZone rightZone, 
            MultiQuantity<IMaterial> rightMaterials)
        {
            Faction = faction;
            LeftZone = leftZone;
            LeftMaterials = leftMaterials;
            RightZone = rightZone;
            RightMaterials = rightMaterials;
        }

        public void Tick()
        {
            if (LeftZone.Contains(LeftMaterials) && RightZone.Contains(RightMaterials))
            {
                LeftZone.Remove(LeftMaterials);
                RightZone.Remove(RightMaterials);
                LeftZone.Add(RightMaterials);
                RightZone.Add(LeftMaterials);
            }
        }
    }
}