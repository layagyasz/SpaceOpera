using Cardamom.Collections;
using Cardamom.Trackers;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Military
{
    public class Unit : DesignedMaterial
    {
        private static readonly EnumSet<ComponentType> s_ArmorTypes = 
            new(ComponentType.PersonalArmor, ComponentType.ShipArmor);
        private static readonly EnumSet<ComponentType> s_ShieldTypes = 
            new(ComponentType.PersonalShield, ComponentType.HeavyShield);
        private static readonly EnumSet<ComponentType> s_WeaponTypes =
            new(ComponentType.SmallArm, ComponentType.HeavyGun, ComponentType.HeavyMissile);

        // Attributes
        public float CargoSpace => GetAttribute(ComponentAttribute.CargoSpace);
        public float Command => GetAttribute(ComponentAttribute.Command);
        public UnitIntervalValue Detection { get; }
        public UnitIntervalValue Evasion { get; }
        public float Hitpoints => GetAttribute(ComponentAttribute.Hitpoints);
        public UnitIntervalValue Maneuver { get; }
        public float MilitaryPower { get; }
        public float Threat => GetAttribute(ComponentAttribute.Threat);

        // Equipment
        public Armor Armor { get; }
        public Shield Shield { get; }
        public MultiCount<Weapon> Weapons { get; }

        public Unit(
            string name, ComponentSlot slot, IEnumerable<ComponentAndSlot> components, MultiCount<ComponentTag> tags)
            : base(name, slot, components, tags)
        {
            Detection = UnitIntervalValue.Of(GetAttribute(ComponentAttribute.Detection));
            Evasion = UnitIntervalValue.Of(GetAttribute(ComponentAttribute.Evasion));
            Maneuver = UnitIntervalValue.Of(GetAttribute(ComponentAttribute.Maneuver));

            Armor = BuildComponent(Components, s_ArmorTypes, Armor.FromComponent, Armor.Combine);
            Shield = BuildComponent(Components, s_ShieldTypes, Shield.FromComponent, Shield.Combine);
            Weapons = 
                components
                    .Where(x => s_WeaponTypes.Contains(x.Component.Slot.Type))
                    .GroupBy(x => x.Component)
                    .ToMultiCount(x => Weapon.FromComponent(x.Key), x => x.Count());

            MilitaryPower = ComputeMilitaryPower();
        }

        public float GetManeuverModifer(UnitIntervalValue tracking)
        {
            return UnitIntervalValue.ToUnitInterval(Math.Max(0, Maneuver.RawValue - tracking.RawValue));
        }

        private float ComputeMilitaryPower()
        {
            // Assume average Tracking of 150.
            var defenseValue = (Shield.MilitaryPower + Armor.MilitaryPower + Hitpoints) 
                / (1f - GetManeuverModifer(UnitIntervalValue.Of(150f)));
            var attackValue = Weapons.Sum(x => x.Value * x.Key.MilitaryPower);
            return GameConstants.MilitaryPower * (0.125f * defenseValue + attackValue);
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
