namespace SpaceOpera.Core.Designs
{
    public record class ComponentAndSlot(DesignSlot Slot, IComponent Component)
    {
        public ComponentAndWeight Resolve(IEnumerable<ComponentAndSlot> components)
        {
            return new(Slot.Weight.Evaluate(Component, components), Component);
        }
    }
}
