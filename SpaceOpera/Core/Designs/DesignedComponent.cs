using Cardamom.Collections;
using Cardamom.Trackers;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Designs
{
    public class DesignedComponent : ComponentBase
    {
        public List<ComponentAndWeight> Components { get; }

        public DesignedComponent(
            string name, ComponentSlot slot, IEnumerable<ComponentAndWeight> components, MultiCount<ComponentTag> tags)
        {
            Name = name;
            Slot = slot;
            Tags = tags;

            Components = components.ToList();
            Attributes = ComputeAttributes(Components);
            MaterialCost = ComputeMaterialCosts(GetAttribute(ComponentAttribute.MaterialCost), Components);
            Damage = ComputeDamage(GetAttribute(ComponentAttribute.WeaponDamage), Components);
            DamageResist = ComputeDamageResist(GetAttribute(ComponentAttribute.DamageResist), Components);
            // Override consumed attributes
            Attributes[ComponentAttribute.MaterialCost] = Modifier.Zero;
            Attributes[ComponentAttribute.WeaponDamage] = Modifier.Zero;
            Attributes[ComponentAttribute.DamageResist] = Modifier.Zero;
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
            float modifier, IEnumerable<ComponentAndWeight> components)
        {
            Dictionary<MaterialReference, MultiQuantity<IMaterial>> referenceMaterials = new();
            foreach (var component in components.Select(x => x.Component))
            {
                foreach (var referenceMaterial in component.ReferenceMaterial)
                {
                    referenceMaterials.Add(referenceMaterial.Key, referenceMaterial.Value);
                }
            }

            Dictionary<IMaterial, Modifier> materials = new();
            foreach (var component in components)
            {
                foreach (var cost in component.Component.ReferenceMaterialCost)
                {
                    foreach (var material in referenceMaterials[cost.Key])
                    {
                        AddCost(materials, material.Key, material.Value * cost.Value);
                    }
                }
                if (component.Component is DesignedMaterial)
                {
                    AddCost(
                        materials, 
                        (IMaterial)component.Component, 
                        new Modifier() { Constant = component.Weight });
                }
                else
                {
                    foreach (var cost in component.Component.MaterialCost)
                    {
                        AddCost(materials, cost.Key, component.Weight * cost.Value);
                    }
                }
            }

            Dictionary<IMaterial, Modifier> finalCost = new();
            foreach (var material in materials)
            {
                finalCost.Add(material.Key, modifier * material.Value.Combine());
            }
            return finalCost;
        }

        private static EnumMap<ComponentAttribute, Modifier> ComputeAttributes(
            IEnumerable<ComponentAndWeight> components)
        {
            return CombineModifiers(1, components.Select(x => (x.Weight, x.Component.Attributes)));
        }

        private static EnumMap<DamageType, Modifier> ComputeDamage(
            float modifier, IEnumerable<ComponentAndWeight> components)
        {
            return CombineModifiers(modifier, components.Select(x => (x.Weight, x.Component.Damage)));
        }

        private static EnumMap<DamageType, Modifier> ComputeDamageResist(
            float modifier, IEnumerable<ComponentAndWeight> components)
        {
            return CombineModifiers(modifier, components.Select(x =>  (x.Weight, x.Component.DamageResist)));
        }

        private static void AddCost(Dictionary<IMaterial, Modifier> materials, IMaterial material, Modifier modifier)
        {
            if (materials.ContainsKey(material))
            {
                materials[material] += modifier;
            }
            else
            {
                materials.Add(material, modifier);
            }
        }

        private static EnumMap<TKey, Modifier> CombineModifiers<TKey>(
            float modifier, IEnumerable<(int, EnumMap<TKey, Modifier>)> maps) where TKey : Enum
        {
            EnumMap<TKey, Modifier> values = new();

            foreach (var map in maps)
            {
                foreach (var entry in map.Item2)
                {
                    values[entry.Key] += map.Item1 * entry.Value;
                }
            }

            foreach (var entry in values)
            {
                values[entry.Key] = modifier * entry.Value.Combine();
            }

            return values;
        }
    }
}