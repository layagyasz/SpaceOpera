using Cardamom.Collections;

namespace SpaceOpera.Core.Designs
{
    public struct DesignSlot
    {
        public EnumSet<ComponentType> Type { get; set; } = new();
        public EnumSet<ComponentSize> Size { get; set; } = new();
        public EnumSet<ComponentTag> RequiredTags { get; set; } = new();
        public int Count { get; set; } = 1;
        public SlotWeight Weight { get; set; } = new();

        public DesignSlot() { }

        public bool Accepts(IComponent component)
        {
            return Type.Contains(component.Slot.Type)
                && (component.Slot.Size == ComponentSize.Unknown
                    || Size.Count == 0
                    || Size.Contains(component.Slot.Size))
                && (RequiredTags.Count == 0 || RequiredTags.All(component.Tags.ContainsKey));
        }

        public override string ToString()
        {
            return string.Format($"[DesignSlot: Type={Type}, Size={Size}, Count={Count}]");
        }
    }
}