using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Military
{
    public readonly struct Shield
    {
        public UnitIntervalValue Absorption { get; }
        public float Capacity { get; }
        public float Recharge { get; }

        public Shield(UnitIntervalValue absorption, float capacity, float recharge)
        {
            Absorption = absorption;
            Capacity = capacity;
            Recharge = recharge;
        }

        public static Shield FromComponent(IComponent component)
        {
            return new Shield(
                new UnitIntervalValue(component.GetAttribute(ComponentAttribute.ShieldAbsorption)),
                component.GetAttribute(ComponentAttribute.ShieldCapacity),
                component.GetAttribute(ComponentAttribute.ShieldRecharge));
        }

        public static Shield Combine(IEnumerable<Shield> armors)
        {
            return new Shield(
                new UnitIntervalValue(armors.Select(x => x.Absorption.RawValue).DefaultIfEmpty(0).Sum()),
                armors.Select(x => x.Capacity).DefaultIfEmpty(0).Sum(),
                armors.Select(x => x.Recharge).DefaultIfEmpty(0).Sum());
        }
    }
}
