using Cardamom.Trackers;
using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Economics
{
    public class DesignedMaterial : DesignedComponent, IMaterial
    {
        public float Mass { get; }
        public float Size { get; }
        public MaterialType Type { get; }
        public float ProductionCost { get; }

        public DesignedMaterial(
            string name, ComponentSlot slot, IEnumerable<ComponentAndSlot> components, MultiCount<ComponentTag> tags)
            : base(name, slot, components, tags)
        {
            Mass = ComputeMass();
            Size = GetAttribute(ComponentAttribute.Size);
            Type = GetMaterialType(slot.Type);
            ProductionCost = GetAttribute(ComponentAttribute.ProductionCost);
        }

        private float ComputeMass()
        {
            return MaterialCost.Sum(x => x.Key.Mass * x.Value.GetTotal());
        }

        private static MaterialType GetMaterialType(ComponentType componentType)
        {
            switch (componentType)
            {
                case ComponentType.BattalionTemplate:
                case ComponentType.DivisionTemplate:
                    return MaterialType.Unknown;
                default:
                    return MaterialType.MaterialDiscrete;
            }
        }
    }
}