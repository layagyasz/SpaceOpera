using Cardamom.Collections;
using Cardamom.Json.Collections;
using Cardamom.Trackers;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Designs
{
    public abstract class ComponentBase : IComponent
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ComponentSlot Slot { get; set; }
        public MultiCount<ComponentTag> Tags { get; set; } = new();
        public EnumMap<MaterialReference, MultiQuantity<IMaterial>> ReferenceMaterial { get; set; } = new();
        public Dictionary<MaterialReference, Modifier> ReferenceMaterialCost { get; set; } = new();

        [JsonConverter(typeof(ReferenceDictionaryJsonConverter))]
        public Dictionary<IMaterial, Modifier> MaterialCost { get; set; } = new();
        public EnumMap<ComponentAttribute, Modifier> Attributes { get; set; } = new();
        public EnumMap<DamageType, Modifier> Damage { get; set; } = new();

        public EnumMap<DamageType, Modifier> DamageResist { get; set; } = new();

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<IAdvancement> Prerequisites { get; set; } = new();

        public float GetAttribute(ComponentAttribute attribute)
        {
            return Attributes[attribute].GetTotal();
        }

        public EnumMap<DamageType, float> GetDamage()
        {
            return TotalModifiers(Damage);
        }

        public EnumMap<DamageType, float> GetDamageResist()
        {
            return TotalModifiers(DamageResist);
        }

        public bool FitsSlot(DesignSlot slot)
        {
            return slot.Type.Contains(Slot.Type)
                && (Slot.Size == ComponentSize.Unknown || slot.Size.Count == 0 || slot.Size.Contains(Slot.Size));
        }

        public override string ToString()
        {
            return $"[Component: Key={Key}]";
        }

        private static EnumMap<TKey, float> TotalModifiers<TKey>(EnumMap<TKey, Modifier> map) where TKey : Enum
        {
            EnumMap<TKey, float> values = new();
            foreach (var entry in map)
            {
                values[entry.Key] = entry.Value.GetTotal();
            }
            return values;
        }
    }
}