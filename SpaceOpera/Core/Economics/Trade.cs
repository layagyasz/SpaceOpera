using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;

namespace SpaceOpera.Core.Economics
{
    public record class Trade(
        EconomicZoneHolding FromZone, EconomicZoneHolding ToZone, MultiQuantity<IMaterial> Materials) : ITickable
    {
        public void Tick()
        {
            // Reimplement using Inventory
        }
    }
}