using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military.Battles
{
    class BattleSide
    {
        public List<IFormation> Formations { get; } = new List<IFormation>();

        public void Add(IFormation Formation)
        {
            Formations.Add(Formation);
        }

        public void Damage(List<DistributedBattleAttack> Attacks, BattleReport.Builder Report)
        {
            foreach (var formation in Attacks.GroupBy(x => x.Attack.TargetFormation))
            {
                float currentCommand = formation.Key.GetCommand();
                float lostCommand = 0;
                foreach (var grouping in formation.GroupBy(x => x.Attack.Target))
                {
                    var group = grouping.Key;
                    if (group.Count <= 0)
                    {
                        continue;
                    }

                    var abs = group.CurrentAbsorption();
                    Damage totalOnTarget = new Damage();
                    float totalEffective = 0;
                    foreach (var attack in grouping)
                    {
                        var onTarget = attack.ComputeOnTarget();
                        var effective = attack.ComputeFinal(1 - abs).GetTotal();
                        totalOnTarget += onTarget;
                        totalEffective += effective;
                        Report
                            .GetBuilderFor(attack.Attack.AttackerFormation.Faction)
                            .GetBuilderFor(attack.Attack.Attacker.Unit)
                            .AddRawDamage(attack.ComputeRaw().GetTotal())
                            .AddOnTargetDamage(onTarget.GetTotal())
                            .AddEffectiveDamage(effective);
                    }
                    var losses = group.DamageHitpoints(totalEffective);
                    group.DamageShield(abs * totalOnTarget);
                    lostCommand += group.Unit.Command * losses;

                    Report
                        .GetBuilderFor(formation.Key.Faction)
                        .GetBuilderFor(group.Unit)
                        .AddLosses(losses)
                        .AddReceivedDamage(totalOnTarget.GetTotal())
                        .AddHullDamage((1 - abs) * totalOnTarget.GetTotal())
                        .AddTakenDamage(totalEffective);
                }
                formation.Key.Cohesion.ChangeAmount(-10 * lostCommand / currentCommand);
            }
        }

        public List<DistributedBattleAttack> GetAttacks(BattleSide OpposingSide, Random Random)
        {
            var result = new List<DistributedBattleAttack>();
            foreach (var formation in Formations)
            {
                foreach (var group in formation.Composition)
                {
                    result.AddRange(
                        DistributedBattleAttack.Generate(GetPotential(formation, group, OpposingSide), Random));
                }
            }
            return result;
        }

        public void Remove(IFormation Formation)
        {
            Formations.Remove(Formation);
        }

        private List<BattleAttack> GetPotential(IFormation Formation, UnitGrouping Unit, BattleSide OpposingSide)
        {
            return Unit.Unit.Weapons.GetCounts()
                .SelectMany(x => GetPotential(Formation, Unit, x, OpposingSide))
                .ToList();
        }

        private IEnumerable<BattleAttack> GetPotential(
            IFormation Formation, UnitGrouping Unit, Count<Weapon> Weapon, BattleSide OpposingSide)
        {
            foreach (var formation in OpposingSide.Formations)
            {
                foreach (var group in formation.Composition)
                {
                    yield return BattleAttack.Create(Unit, Formation, group, formation, Weapon);
                }
            }
        }
    }
}