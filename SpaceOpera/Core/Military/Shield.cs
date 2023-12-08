using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Military
{
    public class Shield
    {
        public UnitIntervalValue Absorption { get; }
        public float Capacity { get; }
        public float Recharge { get; }
        public float MilitaryPower { get; }

        public Shield(UnitIntervalValue absorption, float capacity, float recharge)
        {
            Absorption = absorption;
            Capacity = capacity;
            Recharge = recharge;
            MilitaryPower = ComputeMIlitaryPower();
        }

        private float ComputeMIlitaryPower()
        {
            return Capacity + GameConstants.BattleLength * Recharge;
        }

        public static Shield FromComponent(IComponent component)
        {
            return new Shield(
                UnitIntervalValue.Of(component.GetAttribute(ComponentAttribute.ShieldAbsorption)),
                component.GetAttribute(ComponentAttribute.ShieldCapacity),
                component.GetAttribute(ComponentAttribute.ShieldRecharge));
        }

        public static Shield Combine(IEnumerable<Shield> shields)
        {
            return new Shield(
                UnitIntervalValue.Of(shields.Select(x => x.Absorption.RawValue).DefaultIfEmpty(0).Sum()),
                shields.Select(x => x.Capacity).DefaultIfEmpty(0).Sum(),
                shields.Select(x => x.Recharge).DefaultIfEmpty(0).Sum());
        }
    }
}
