using SpaceOpera.Core.Designs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military
{
    class Weapon
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
                new UnitIntervalValue(Component.GetAttribute(ComponentAttribute.WEAPON_ACCURACY)), 
                new UnitIntervalValue(Component.GetAttribute(ComponentAttribute.WEAPON_TRACKING)), 
                Damage.FromComponent(Component),
                Component.GetAttribute(ComponentAttribute.WEAPON_PENETRATION));
        }

        public override string ToString()
        {
            return string.Format(
                "[Weapon: Name={0}, Accuracy={1}, Tracking={2}, Damage={3}]", Name, Accuracy, Tracking, Damage);
        }
    }
}