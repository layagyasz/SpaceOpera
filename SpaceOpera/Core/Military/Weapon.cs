using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Military
{
    public class Weapon
    {
        public string Name { get; }
        public UnitIntervalValue Accuracy { get; }
        public UnitIntervalValue Tracking { get; }
        public Damage Damage { get; }
        public float Penetration { get; }

        public Weapon(
            string Name, UnitIntervalValue Accuracy, UnitIntervalValue Tracking, Damage Damage, float Penetration)
        {
            this.Name = Name;
            this.Accuracy = Accuracy;
            this.Tracking = Tracking;
            this.Damage = Damage;
            this.Penetration = Penetration;
        }

        public static Weapon FromComponent(IComponent Component)
        {
            return new Weapon(
                Component.Name,
                new UnitIntervalValue(Component.GetAttribute(ComponentAttribute.WeaponAccuracy)), 
                new UnitIntervalValue(Component.GetAttribute(ComponentAttribute.WeaponTracking)), 
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