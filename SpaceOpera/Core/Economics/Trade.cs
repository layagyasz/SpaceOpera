using SpaceOpera.Core.Advanceable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Economics
{
    class Trade : ITickable
    {
        public EconomicZone LeftZone { get; }
        public MultiQuantity<IMaterial> LeftMaterials { get; }

        public EconomicZone RightZone { get; set; }
        public MultiQuantity<IMaterial> RightMaterials { get; }

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