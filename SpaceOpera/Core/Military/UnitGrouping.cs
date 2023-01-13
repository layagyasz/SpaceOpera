using SpaceOpera.Core.Military.Battles;

namespace SpaceOpera.Core.Military
{
    public class UnitGrouping
    {
        private static readonly float s_ShieldConstant = 12;

        public Unit Unit { get; }
        public int Count { get; private set; }
        public double Shielding { get; private set; }

        public UnitGrouping(Unit Unit, int Count)
        {
            this.Unit = Unit;
            this.Count = Count;

            Shielding = Count * Unit.Shield.Capacity;
        }

        public void Recharge()
        {
            Shielding = Math.Min(Shielding + Count * Unit.Shield.Recharge, Count * Unit.Shield.Capacity);
        }

        private double CurrentAbsorption()
        {
            if (Shielding < double.Epsilon)
            {
                return 0;
            }
            var v = Unit.Shield.Absorption * Shielding / (Count * Unit.Shield.Capacity);
            return v / (v + s_ShieldConstant);
        }


        private void DamageShield(double Damage)
        {
            Shielding = Math.Max(0, Shielding - Damage);
        }

        private void TakeCasualties(int Casualties)
        {
            Count = Math.Max(0, Count - Casualties);
        }

        public void Damage(IEnumerable<DistributedBattleAttack> Attacks, Random Random)
        {
            double rawDamage = Attacks.Sum(x => x.ComputeRaw());
            DamageShield(CurrentAbsorption() * rawDamage);

            double finalDamage = Attacks.Sum(x => x.ComputeFinal(1 - CurrentAbsorption()));
            TakeCasualties(ToCasualtyNumber(finalDamage, Unit, Random));
        }

        private static int ToCasualtyNumber(double Damage, Unit Target, Random Random)
        {
            double damage = 1 / Target.Hitpoints * Damage;
            int additional = Random.NextDouble() < Math.Floor(damage) - damage ? 1 : 0;
            return (int)Math.Floor(damage) + additional;
        }

        public override string ToString()
        {
            return string.Format("[UnitGrouping Unit={0}, Shield={1}, Count={2}]", Unit, Shielding, Count);
        }
    }
}