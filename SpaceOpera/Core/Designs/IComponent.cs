using Cardamom;
using Cardamom.Collections;
using Cardamom.Trackers;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Designs
{
    public interface IComponent : IKeyed
    {
        string Name { get; }
        ComponentSlot Slot { get; }
        List<ComponentTag> Tags { get; }
        EnumMap<MaterialReference, MultiQuantity<IMaterial>> ReferenceMaterial { get; }
        Dictionary<MaterialReference, Modifier> ReferenceMaterialCost { get; }
        Dictionary<IMaterial, Modifier> MaterialCost { get; }
        EnumMap<ComponentAttribute, Modifier> Attributes { get; }
        EnumMap<DamageType, Modifier> Damage { get; }

        EnumMap<DamageType, Modifier> DamageResist { get; }
        List<IAdvancement> Prerequisites { get; }

        bool FitsSlot(DesignSlot slot);
        float GetAttribute(ComponentAttribute attribute);
        EnumMap<DamageType, float> GetDamage();
    }
}
