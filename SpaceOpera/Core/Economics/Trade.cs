using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;

namespace SpaceOpera.Core.Economics
{
    public record class Trade(
        EconomicZone FromZone, EconomicZone ToZone, MultiQuantity<IMaterial> Materials) : ITickable
    {
        public void Tick()
        {
            // Reimplement using Inventory
        }
    }
}