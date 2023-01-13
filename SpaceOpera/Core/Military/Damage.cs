using Cardamom.Collections;
using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Military
{
    public class Damage
    {
        public EnumMap<DamageType, float> Amount { get; }

        public Damage()
        {
            Amount = new();
        }

        public Damage(EnumMap<DamageType, float> amount)
        {
            Amount = amount;
        }

        public static Damage FromComponent(IComponent component)
        {
            return new Damage(component.GetDamage());
        }

        public Damage Cap(float maximum)
        {
            var r = maximum / GetTotal();
            return r < 1 ? r * this : this;
        }

        public float GetTotal()
        {
            return Amount.Sum(x => x.Value);
        }

        public static Damage operator +(Damage left, Damage right)
        {
            var newDamage = new EnumMap<DamageType, float>();
            foreach (var type in Enum.GetValues(typeof(DamageType)))
            {
                newDamage.Add((DamageType)type, left.Amount[(DamageType)type] + right.Amount[(DamageType)type]);
            }
            return new Damage(newDamage);
        }

        public static Damage operator -(Damage damage, float reduction)
        {
            return (1 - reduction / damage.GetTotal()) * damage;
        }

        public static Damage operator *(float scale, Damage damage)
        {
            var newDamage = new EnumMap<DamageType, float>();
            foreach (var d in damage.Amount)
            {
                newDamage.Add(d.Key, scale * d.Value);
            }
            return new Damage(newDamage);
        }
    }
}