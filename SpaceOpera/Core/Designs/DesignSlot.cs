using Cardamom.Collections;

namespace SpaceOpera.Core.Designs
{
    public struct DesignSlot
    {
        public EnumSet<ComponentType> Type { get; set; } = new();
        public EnumSet<ComponentSize> Size { get; set; } = new();
        public int Count { get; set; }
        public int Weight { get; set; }

        public DesignSlot() { }
    }
}