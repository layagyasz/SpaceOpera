using SpaceOpera.Core.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    class DesignedComponent : ComponentBase
    {
        public List<ComponentAndSlot> Components { get; }

        public DesignedComponent(
            string Name, ComponentSlot Slot, IEnumerable<ComponentAndSlot> Components, IEnumerable<ComponentTag> Tags)
        {
            this.Name = Name;
            this.Slot = Slot;
            this.Tags = Tags.ToList();

            this.Components = Components.ToList();
            this.Attributes = ComputeAttributes(Components);
            this.MaterialCost = ComputeMaterialCosts(GetAttribute(ComponentAttribute.MATERIAL_COST), Components);
            this.Damage = ComputeDamage(GetAttribute(ComponentAttribute.WEAPON_DAMAGE), Components);
            this.DamageResist = ComputeDamageResist(GetAttribute(ComponentAttribute.DAMAGE_RESIST), Components);
        }

        public MultiQuantity<IMaterial> GetMaterialCost()
        {
            var cost = new MultiQuantity<IMaterial>();
            foreach (var material in MaterialCost)
            {
                cost.Add(material.Key, material.Value.GetTotal());
            }
            return cost;
        }

        protected virtual Dictionary<IMaterial, Modifier> ComputeMaterialCosts(
            float Modifier, IEnumerable<ComponentAndSlot> Components)
        {
            Dictionary<MaterialReference, MultiQuantity<IMaterial>> referenceMaterials =
                new Dictionary<MaterialReference, MultiQuantity<IMaterial>>();
            foreach (var component in Components.Select(x => x.Component))
            {
                if (component.ReferenceMaterial != null)
                {
                    foreach (var referenceMaterial in component.ReferenceMaterial)
                    {
                        referenceMaterials.Add(referenceMaterial.Key, referenceMaterial.Value);
                    }
                }
            }

            Dictionary<IMaterial, Modifier> materials = new Dictionary<IMaterial, Modifier>();
            foreach (var component in Components)
            {
                if (component.Component.ReferenceMaterialCost != null)
                {
                    foreach (var cost in component.Component.ReferenceMaterialCost)
                    {
                        foreach (var material in referenceMaterials[cost.Key])
                        {
                            AddCost(materials, material.Key, material.Value * cost.Value);
                        }
                    }
                }
                if (component.Component.MaterialCost != null)
                {
                    if (component.Component is DesignedMaterial)
                    {
                        AddCost(
                            materials, 
                            (IMaterial)component.Component, 
                            new Modifier() { Constant = component.Slot.Weight });
                    }
                    else
                    {
                        foreach (var cost in component.Component.MaterialCost)
                        {
                            AddCost(materials, cost.Key, component.Slot.Weight * cost.Value);
                        }
                    }
                }
            }

            Dictionary<IMaterial, Modifier> finalCost = new Dictionary<IMaterial, Modifier>();
            foreach (var material in materials)
            {
                finalCost.Add(material.Key, Modifier * material.Value.Combine());
            }
            return finalCost;
        }

        private static EnumMap<ComponentAttribute, Modifier> ComputeAttributes(
            IEnumerable<ComponentAndSlot> Components)
        {
            return CombineModifiers<ComponentAttribute>(1, Components.Select(x => x.Component.Attributes));
        }

        private static EnumMap<DamageType, Modifier> ComputeDamage(
            float Modifier, IEnumerable<ComponentAndSlot> Components)
        {
            return CombineModifiers<DamageType>(Modifier, Components.Select(x => x.Component.Damage));
        }

        private static EnumMap<DamageType, Modifier> ComputeDamageResist(
            float Modifier, IEnumerable<ComponentAndSlot> Components)
        {
            return CombineModifiers<DamageType>(Modifier, Components.Select(x =>  x.Component.DamageResist));
        }

        private static void AddCost(Dictionary<IMaterial, Modifier> Materials, IMaterial Material, Modifier Modifier)
        {
            if (Materials.ContainsKey(Material))
            {
                Materials[Material] += Modifier;
            }
            else
            {
                Materials.Add(Material, Modifier);
            }
        }

        private static EnumMap<TKey, Modifier> CombineModifiers<TKey>(
            float Modifier, IEnumerable<EnumMap<TKey, Modifier>> Maps)
            where TKey : struct, IConvertible
        {
            EnumMap<TKey, Modifier> values = new EnumMap<TKey, Modifier>();

            foreach (var map in Maps)
            {
                if (map == null)
                {
                    continue;
                }
                foreach (var entry in map)
                {
                    values[entry.Key] += entry.Value;
                }
            }

            foreach (var entry in values)
            {
                values[entry.Key] = Modifier * entry.Value.Combine();
            }

            return values;
        }
    }
}