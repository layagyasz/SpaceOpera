namespace SpaceOpera.Core.Designs
{
    public class ComponentAndSlot
    {
        public DesignSlot Slot { get; }
        public IComponent Component { get; }

        public ComponentAndSlot(DesignSlot slot, IComponent component)
        {
            Slot = slot;
            Component = component;
        }
    }
}
