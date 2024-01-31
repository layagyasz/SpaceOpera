namespace SpaceOpera.Core.Designs
{
    public struct SlotWeight
    {
        public struct Part
        {
            public ComponentAttribute Attribute { get; set; }
            public float Coefficient { get; set; } = 1;

            public Part() { }

            public float Evaluate(IComponent component)
            {
                return Attribute == ComponentAttribute.Unknown
                    ? Coefficient : Coefficient * component.GetAttribute(Attribute);
            }
        }

        public ComponentSlot TargetSlot { get; set; }
        public Part Target { get; set; } = new();
        public Part Divisor { get; set; } = new();

        public SlotWeight() { }

        public int Evaluate(IComponent component, IEnumerable<ComponentAndSlot> designComponents)
        {
            var targetSlot = TargetSlot;
            var targetPart = Target;
            var target = 
                TargetSlot.Type == ComponentType.Unknown 
                ? Target.Coefficient 
                : Target.Coefficient * designComponents.Where(x => x.Component.Slot == targetSlot)
                    .Sum(x => x.Resolve(designComponents).Weight * targetPart.Evaluate(x.Component));
            return (int)Math.Ceiling(target / Divisor.Evaluate(component));
        }
    }
}
