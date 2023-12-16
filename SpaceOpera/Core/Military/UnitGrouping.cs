using Cardamom;
using Cardamom.Trackers;

namespace SpaceOpera.Core.Military
{
    public class UnitGrouping
    {
        public Unit Unit { get; }
        public IntPool Count { get; }
        public Pool Hitpoints { get; }
        public Pool Shielding { get; }

        public UnitGrouping(Count<Unit> unit)
        {
            Unit = unit.Key;
            Count = new(unit.Value);

            Hitpoints = new(unit.Value * Unit.Hitpoints);
            Shielding = new(unit.Value * Unit.Shield.Capacity);
        }

        public void Combine(UnitGrouping other)
        {
            Precondition.Check(other.Unit == Unit);
            Count.Merge(other.Count);
            Hitpoints.Merge(other.Hitpoints);
            Shielding.Merge(other.Shielding);
        }

        public void Recharge()
        {
            Shielding.Change(Count.Amount * Unit.Shield.Recharge);
        }

        public float GetCurrentAbsorption()
        {
            return UnitIntervalValue.ToUnitInterval(Unit.Shield.Absorption.RawValue * Shielding.PercentFull());
        }

        public int DamageHitpoints(float damage)
        {
            Hitpoints.Change(-damage);
            var p = 1 - Hitpoints.Amount / (Count.MaxAmount * Unit.Hitpoints);
            int losses = Count.Amount - (int)(Count.MaxAmount * (1 - p * p));
            TakeCasualties(losses);
            return losses;
        }

        public void DamageShield(Damage damage)
        {
            Shielding.Change(-damage.GetTotal());
        }

        public float GetMilitaryPower()
        {
            return Count.Amount * Unit.MilitaryPower;
        }

        private void TakeCasualties(int casualties)
        {
            Count.Change(-casualties);
            Hitpoints.ChangeMax(-casualties * Unit.Hitpoints);
            Shielding.ChangeMax(-casualties * Unit.Shield.Capacity);
        }

        public override string ToString()
        {
            return string.Format(
                "[UnitGrouping: Unit={0}, Count={1}, Hitpoints={2}, Shielding={3}]", 
                Unit, 
                Count,
                Hitpoints, 
                Shielding);
        }
    }
}