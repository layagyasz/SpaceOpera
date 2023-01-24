using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics
{
    public class SingleMaterialSink
    {
        public MultiQuantity<IMaterial> Materials { get; set; } = new();
    }
}
