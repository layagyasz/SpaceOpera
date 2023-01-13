using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;

namespace SpaceOpera.Core.Economics
{
    class Trade : ITickable
    {
        public EconomicZone LeftZone { get; }
        public MultiQuantity<IMaterial> LeftMaterials { get; }

        public EconomicZone RightZone { get; set; }
        public MultiQuantity<IMaterial> RightMaterials { get; }

        public Trade(
            EconomicZone leftZone,
            MultiQuantity<IMaterial> leftMaterials, 
            EconomicZone rightZone,
            MultiQuantity<IMaterial> rightMaterials)
        {
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