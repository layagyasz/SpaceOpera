using Cardamom;

namespace SpaceOpera.Core.Designs
{
    public class SegmentConfiguration : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public IComponent? IntrinsicComponent { get; set; }
        public List<DesignSlot> Slots { get; set; } = new();
    }
}
