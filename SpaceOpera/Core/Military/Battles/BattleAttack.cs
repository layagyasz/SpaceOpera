using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military.Battles
{
    class BattleAttack
    {
        public UnitGrouping Attacker { get; }
        public IFormation AttackerFormation { get; }
        public UnitGrouping Target { get; }
        public IFormation TargetFormation { get; }
        public Count<Weapon> Weapon { get; }

        private BattleAttack(
            UnitGrouping Attacker,
            IFormation AttackerFormation, 
            UnitGrouping Target,
            IFormation TargetFormation, 
            Count<Weapon> Weapon)
        {
            this.Attacker = Attacker;
            this.AttackerFormation = AttackerFormation;
            this.Target = Target;
            this.TargetFormation = TargetFormation;
            this.Weapon = Weapon;
        }

        public static BattleAttack Create(
            UnitGrouping Attacker,
            IFormation AttackerFormation,
            UnitGrouping Target, 
            IFormation TargetFormation, 
            Count<Weapon> Weapon)
        {
            return new BattleAttack(Attacker, AttackerFormation, Target, TargetFormation, Weapon);
        }

        public Damage ComputePotential()
        {
            return Weapon.Amount * ComputePotentialImpl(Weapon.Value, Target.Unit, 1);
        }

        public Damage ComputeRaw()
        {
            return Weapon.Amount * Weapon.Value.Damage;
        }

        public Damage ComputeOnTarget()
        {
            return Weapon.Amount * ComputeUnadjustedPotentialImpl(Weapon.Value, Target.Unit);
        }

        public Damage ComputeFinal(float Adjustment)
        {
            return Weapon.Amount * ComputePotentialImpl(Weapon.Value, Target.Unit, Adjustment);
        }

        private static Damage ComputePotentialImpl(Weapon Weapon, Unit Target, float Adjustment)
        {
            var unadjustedDamage = Adjustment * ComputeUnadjustedPotentialImpl(Weapon, Target);
            Damage adjustedDamage;
            if (Adjustment * Weapon.Penetration > Target.Armor.Thickness)
            {
                adjustedDamage = unadjustedDamage;
            }
            else if (unadjustedDamage.GetTotal() > Target.Armor.Protection)
            {
                adjustedDamage = Target.Armor.Coverage * (unadjustedDamage - Target.Armor.Protection) 
                    + (1 - Target.Armor.Coverage) * unadjustedDamage;
            }
            else
            {
                adjustedDamage = (1 - Target.Armor.Coverage) * unadjustedDamage;
            }
            return adjustedDamage.Cap(Target.Hitpoints);
        }

        private static Damage ComputeUnadjustedPotentialImpl(Weapon Weapon, Unit Target)
        {
            return
                Math.Max(
                    0, Weapon.Accuracy.UnitValue - Math.Max(0, Target.Maneuver.UnitValue - Weapon.Tracking.UnitValue))
                * Weapon.Damage;
        }

        public override string ToString()
        {
            return string.Format("[BattleAttack: Weapon={0}]", Weapon);
        }
    }
}