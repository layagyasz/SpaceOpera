using SpaceOpera.Core.Designs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military
{
    class Damage
    {
        public EnumMap<DamageType, float> Amount { get; }

        public Damage()
        {
            this.Amount = new EnumMap<DamageType, float>();
        }

        public Damage(EnumMap<DamageType, float> Amount)
        {
            this.Amount = Amount;
        }

        public static Damage FromComponent(IComponent Component)
        {
            return new Damage(Component.GetDamage());
        }

        public Damage Cap(float Maximum)
        {
            var r = Maximum / GetTotal();
            return r < 1 ? r * this : this;
        }

        public float GetTotal()
        {
            return Amount.Sum(x => x.Value);
        }

        public static Damage operator +(Damage Left, Damage Right)
        {
            var newDamage = new EnumMap<DamageType, float>();
            foreach (var type in Enum.GetValues(typeof(DamageType)))
            {
                newDamage.Add((DamageType)type, Left.Amount[(DamageType)type] + Right.Amount[(DamageType)type]);
            }
            return new Damage(newDamage);
        }

        public static Damage operator -(Damage Damage, float Reduction)
        {
            return (1 - Reduction / Damage.GetTotal()) * Damage;
        }

        public static Damage operator *(float Scale, Damage Damage)
        {
            var newDamage = new EnumMap<DamageType, float>();
            foreach (var damage in Damage.Amount)
            {
                newDamage.Add(damage.Key, Scale * damage.Value);
            }
            return new Damage(newDamage);
        }
    }
}