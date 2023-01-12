using SpaceOpera.Core.Designs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceOpera.Core.Economics
{
    class DesignedMaterial : DesignedComponent, IMaterial
    {
        public double Mass { get; }
        public double Size { get; }
        public MaterialType Type { get; }
        public float ProductionCost { get; }

        public DesignedMaterial(
            string Name, ComponentSlot Slot, IEnumerable<ComponentAndSlot> Components, IEnumerable<ComponentTag> Tags)
            : base(Name, Slot, Components, Tags)
        {
            this.Mass = ComputeMass();
            this.Size = GetAttribute(ComponentAttribute.Size);
            this.Type = GetMaterialType(Slot.Type);
            this.ProductionCost = GetAttribute(ComponentAttribute.ProductionCost);
        }

        private double ComputeMass()
        {
            return MaterialCost.Sum(x => x.Key.Mass * x.Value.GetTotal());
        }

        private static MaterialType GetMaterialType(ComponentType ComponentType)
        {
            switch (ComponentType)
            {
                case ComponentType.BattalionTemplate:
                case ComponentType.DivisionTemplate:
                    return MaterialType.NONE;
                default:
                    return MaterialType.MATERIAL_DISCRETE;
            }
        }
    }
}