namespace SpaceOpera.Core.Designs
{
    public class SegmentConfiguration
    {
        public IComponent? IntrinsicComponent { get; set; }
        public List<DesignSlot> Slots { get; set; } = new();
    }
}
