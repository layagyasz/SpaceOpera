using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Military
{
    public class Weapon
    {
        public string Name { get; }
        public UnitIntervalValue Accuracy { get; }
        public UnitIntervalValue Tracking { get; }
        public Damage Damage { get; }
        public float MilitaryPower { get; }
        public float Penetration { get; }

        public Weapon(
            string name, UnitIntervalValue accuracy, UnitIntervalValue tracking, Damage damage, float penetration)
        {
            Name = name;
            Accuracy = accuracy;
            Tracking = tracking;
            Damage = damage;
            Penetration = penetration;
            MilitaryPower = ComputeMilitaryPower();
        }

        private float ComputeMilitaryPower()
        {
            // Assume average Maneuver of 150.
            return Math.Max(
                0, Accuracy.UnitValue - UnitIntervalValue.ToUnitInterval(Math.Max(0, 150f - Tracking.RawValue)))
                * Damage.GetTotal() * MathF.Sqrt(Penetration);
        }

        public static Weapon FromComponent(IComponent Component)
        {
            return new Weapon(
                Component.Name,
                UnitIntervalValue.Of(Component.GetAttribute(ComponentAttribute.WeaponAccuracy)), 
                UnitIntervalValue.Of(Component.GetAttribute(ComponentAttribute.WeaponTracking)), 
                Damage.FromComponent(Component),
                Component.GetAttribute(ComponentAttribute.WeaponPenetration));
        }

        public override string ToString()
        {
            return string.Format(
                "[Weapon: Name={0}, Accuracy={1}, Tracking={2}, Damage={3}]", Name, Accuracy, Tracking, Damage);
        }
    }
}