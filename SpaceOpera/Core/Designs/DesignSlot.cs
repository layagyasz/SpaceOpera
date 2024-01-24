using Cardamom.Collections;

namespace SpaceOpera.Core.Designs
{
    public struct DesignSlot
    {
        public EnumSet<ComponentType> Type { get; set; } = new();
        public EnumSet<ComponentSize> Size { get; set; } = new();
        public int Count { get; set; } = 1;
        public SlotWeight Weight { get; set; } = new();

        public DesignSlot() { }

        public bool Accepts(ComponentSlot slot)
        {
            return Type.Contains(slot.Type)
                && (slot.Size == ComponentSize.Unknown || Size.Count == 0 || Size.Contains(slot.Size));
        }

        public override string ToString()
        {
            return string.Format($"[DesignSlot: Type={Type}, Size={Size}, Count={Count}]");
        }
    }
}