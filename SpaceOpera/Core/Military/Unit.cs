using Cardamom.Collections;
using Cardamom.Trackers;
using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Military
{
    public class Unit : DesignedComponent
    {
        private static readonly EnumSet<ComponentType> s_ArmorTypes = 
            new(ComponentType.PersonalArmor, ComponentType.ShipArmor);
        private static readonly EnumSet<ComponentType> s_ShieldTypes = 
            new(ComponentType.PersonalShield, ComponentType.ShipShield);
        private static readonly EnumSet<ComponentType> s_WeaponTypes =
            new(ComponentType.PersonalWeapon, ComponentType.ShipWeapon, ComponentType.ShipMissile);

        // Attributes
        public float Command => GetAttribute(ComponentAttribute.Command);
        public UnitIntervalValue Detection { get; }
        public UnitIntervalValue Evasion { get; }
        public float Hitpoints => GetAttribute(ComponentAttribute.Hitpoints);
        public UnitIntervalValue Maneuver { get; }
        public float Threat => GetAttribute(ComponentAttribute.Threat);

        // Equipment
        public Armor Armor { get; }
        public Shield Shield { get; }
        public MultiCount<Weapon> Weapons { get; }

        public Unit(
            string name, ComponentSlot slot, IEnumerable<ComponentAndSlot> components, IEnumerable<ComponentTag> tags)
            : base(name, slot, components, tags)
        {
            Detection = new UnitIntervalValue(GetAttribute(ComponentAttribute.Detection));
            Evasion = new UnitIntervalValue(GetAttribute(ComponentAttribute.Evasion));
            Maneuver = new UnitIntervalValue(GetAttribute(ComponentAttribute.Maneuver));

            Armor = BuildComponent(Components, s_ArmorTypes, Armor.FromComponent, Armor.Combine);
            Shield = BuildComponent(Components, s_ShieldTypes, Shield.FromComponent, Shield.Combine);
            Weapons = components
                .Where(x => s_WeaponTypes.Contains(x.Component.Slot.Type))
                .Select(x => Count<Weapon>.Create(Weapon.FromComponent(x.Component), x.Slot.Count))
                .ToMultiCount(x => x.Key, x => x.Value);
        }

        private static T BuildComponent<T>(
            IEnumerable<ComponentAndSlot> components, 
            EnumSet<ComponentType> componentTypes,
            Func<IComponent, T> builder, 
            Func<IEnumerable<T>, T> aggregator)
        {
            return aggregator(
                components
                    .Where(x => componentTypes.Contains(x.Component.Slot.Type))
                    .SelectMany(x => Enumerable.Repeat(builder(x.Component), x.Slot.Count)));
        }
    }
}
