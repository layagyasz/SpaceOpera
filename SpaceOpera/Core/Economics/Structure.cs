using Cardamom;
using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics
{
    public class Structure : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public uint MaxWorkers { get; set; }
        public uint BuildTime { get; set; }
        public MultiQuantity<IMaterial> Cost { get; set; } = new();

        public override string ToString()
        {
            return string.Format("[Structure: Key={0}]", Key);
        }
    }
}