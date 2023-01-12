namespace SpaceOpera.Core.Designs
{
    public class ComponentAndSlot
    {
        public IComponent Component { get; }
        public DesignSlot Slot { get; }

        public ComponentAndSlot(IComponent component, DesignSlot slot)
        {
            Component = component;
            Slot = slot;
        }
    }
}
