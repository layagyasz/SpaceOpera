using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Economics;
using SpaceOpera.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    class ComponentBase : IComponent
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public ComponentSlot Slot { get; set; }
        public List<ComponentTag> Tags { get; set; }

        [JsonConverter(typeof(EnumMapJsonConverter<MaterialReference, MultiQuantity<IMaterial>>))]
        public EnumMap<MaterialReference, MultiQuantity<IMaterial>> ReferenceMaterial { get; set; }
        public Dictionary<MaterialReference, Modifier> ReferenceMaterialCost { get; set; }
        [JsonConverter(typeof(DictionaryJsonConverter<IMaterial, Modifier>))]
        public Dictionary<IMaterial, Modifier> MaterialCost { get; set; }

        [JsonConverter(typeof(EnumMapJsonConverter<ComponentAttribute, Modifier>))]
        public EnumMap<ComponentAttribute, Modifier> Attributes { get; set; }

        [JsonConverter(typeof(EnumMapJsonConverter<DamageType, Modifier>))]
        public EnumMap<DamageType, Modifier> Damage { get; set; }

        [JsonConverter(typeof(EnumMapJsonConverter<DamageType, Modifier>))]
        public EnumMap<DamageType, Modifier> DamageResist { get; set; }
        public List<IAdvancement> Prerequisites { get; set; }

        public float GetAttribute(ComponentAttribute Attribute)
        {
            if (Attributes == null)
            {
                return 0;
            }
            return Attributes[Attribute].GetTotal();
        }

        public EnumMap<DamageType, float> GetDamage()
        {
            return TotalModifiers(Damage);
        }

        public EnumMap<DamageType, float> GetDamageResist()
        {
            return TotalModifiers(DamageResist);
        }

        public bool FitsSlot(DesignSlot Slot)
        {
            return Slot.Type.Contains(this.Slot.Type)
                && (this.Slot.Size == ComponentSize.NONE
                || Slot.Size.Count == 0
                || Slot.Size.Contains(this.Slot.Size));
        }

        private static EnumMap<TKey, float> TotalModifiers<TKey>(EnumMap<TKey, Modifier> Map)
            where TKey : struct, IConvertible
        {
            EnumMap<TKey, float> values = new EnumMap<TKey, float>();
            foreach (var entry in Map)
            {
                values[entry.Key] = entry.Value.GetTotal();
            }
            return values;
        }
    }
}