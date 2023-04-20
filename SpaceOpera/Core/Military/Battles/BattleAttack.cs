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
            return Weapon.Value 
                * ComputeOnTargetImpl(Weapon.Key, Target.Unit) 
                * ComputeEffectiveImpl(Weapon.Key, Target.Unit, 1);
        }

        public Damage ComputeRaw()
        {
            return Weapon.Value * Weapon.Key.Damage;
        }

        public Damage ComputeOnTarget()
        {
            return Weapon.Value * ComputeOnTargetImpl(Weapon.Key, Target.Unit) * Weapon.Key.Damage;
        }

        public Damage ComputeFinal(float adjustment)
        {
            return Weapon.Value
                * ComputeOnTargetImpl(Weapon.Key, Target.Unit) 
                * ComputeEffectiveImpl(Weapon.Key, Target.Unit, adjustment);
        }

        private static Damage ComputeEffectiveImpl(Weapon weapon, Unit target, float adjustment)
        {
            var unadjustedDamage = weapon.Damage;
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

        private static float ComputeOnTargetImpl(Weapon weapon, Unit target)
        {
            return
                Math.Max(
                    0, weapon.Accuracy.UnitValue - Math.Max(0, target.Maneuver.UnitValue - weapon.Tracking.UnitValue));
        }

        public override string ToString()
        {
            return string.Format("[BattleAttack: Weapon={0}]", Weapon);
        }
    }
}