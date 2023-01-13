using Cardamom.Trackers;

namespace SpaceOpera.Core.Military.Battles
{
    public class BattleAttack
    {
        public UnitGrouping Attacker { get; }
        public IFormation AttackerFormation { get; }
        public UnitGrouping Target { get; }
        public IFormation TargetFormation { get; }
        public Count<Weapon> Weapon { get; }

        private BattleAttack(
            UnitGrouping attacker,
            IFormation attackerFormation, 
            UnitGrouping target,
            IFormation targetFormation, 
            Count<Weapon> weapon)
        {
            Attacker = attacker;
            AttackerFormation = attackerFormation;
            Target = target;
            TargetFormation = targetFormation;
            Weapon = weapon;
        }

        public static BattleAttack Create(
            UnitGrouping attacker,
            IFormation attackerFormation,
            UnitGrouping target, 
            IFormation targetFormation, 
            Count<Weapon> weapon)
        {
            return new BattleAttack(attacker, attackerFormation, target, targetFormation, weapon);
        }

        public Damage ComputePotential()
        {
            return Weapon.Value * ComputePotentialImpl(Weapon.Key, Target.Unit, 1);
        }

        public Damage ComputeRaw()
        {
            return Weapon.Value * Weapon.Key.Damage;
        }

        public Damage ComputeOnTarget()
        {
            return Weapon.Value * ComputeUnadjustedPotentialImpl(Weapon.Key, Target.Unit);
        }

        public Damage ComputeFinal(float adjustment)
        {
            return Weapon.Value * ComputePotentialImpl(Weapon.Key, Target.Unit, adjustment);
        }

        private static Damage ComputePotentialImpl(Weapon weapon, Unit target, float adjustment)
        {
            var unadjustedDamage = adjustment * ComputeUnadjustedPotentialImpl(weapon, target);
            Damage adjustedDamage;
            if (adjustment * weapon.Penetration > target.Armor.Thickness)
            {
                adjustedDamage = unadjustedDamage;
            }
            else if (unadjustedDamage.GetTotal() > target.Armor.Protection)
            {
                adjustedDamage = target.Armor.Coverage * (unadjustedDamage - target.Armor.Protection) 
                    + (1 - target.Armor.Coverage) * unadjustedDamage;
            }
            else
            {
                adjustedDamage = (1 - target.Armor.Coverage) * unadjustedDamage;
            }
            return adjustedDamage.Cap(target.Hitpoints);
        }

        private static Damage ComputeUnadjustedPotentialImpl(Weapon weapon, Unit target)
        {
            return
                Math.Max(
                    0, weapon.Accuracy.UnitValue - Math.Max(0, target.Maneuver.UnitValue - weapon.Tracking.UnitValue))
                * weapon.Damage;
        }

        public override string ToString()
        {
            return string.Format("[BattleAttack: Weapon={0}]", Weapon);
        }
    }
}