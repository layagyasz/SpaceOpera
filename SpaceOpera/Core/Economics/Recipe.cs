using Cardamom;
using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics
{
    public class Recipe : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Structure? Structure { get; set; }
        public float Coefficient { get; set; }
        public MultiQuantity<IMaterial> Transformation { get; set; } = new();
        public IMaterial? BoundResourceNode { get; set; }
    }
}
