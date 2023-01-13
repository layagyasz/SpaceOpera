using Cardamom.Trackers;

namespace SpaceOpera.Core.Military.Battles
{
    public class BattleSide
    {
        public List<IFormation> Formations { get; } = new();

        public void Add(IFormation Formation)
        {
            Formations.Add(Formation);
        }

        public void Damage(List<DistributedBattleAttack> attacks, BattleReport.Builder report)
        {
            foreach (var formation in attacks.GroupBy(x => x.Attack.TargetFormation))
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
                        report
                            .GetBuilderFor(attack.Attack.AttackerFormation.Faction)
                            .GetBuilderFor(attack.Attack.Attacker.Unit)
                            .AddRawDamage(attack.ComputeRaw().GetTotal())
                            .AddOnTargetDamage(onTarget.GetTotal())
                            .AddEffectiveDamage(effective);
                    }
                    var losses = group.DamageHitpoints(totalEffective);
                    group.DamageShield(abs * totalOnTarget);
                    lostCommand += group.Unit.Command * losses;

                    report
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

        public List<DistributedBattleAttack> GetAttacks(BattleSide opposingSide, Random random)
        {
            var result = new List<DistributedBattleAttack>();
            foreach (var formation in Formations)
            {
                foreach (var group in formation.Composition)
                {
                    result.AddRange(
                        DistributedBattleAttack.Generate(GetPotential(formation, group, opposingSide), random));
                }
            }
            return result;
        }

        public void Remove(IFormation formation)
        {
            Formations.Remove(formation);
        }

        private List<BattleAttack> GetPotential(IFormation formation, UnitGrouping unit, BattleSide opposingSide)
        {
            return unit.Unit.Weapons.GetCounts()
                .SelectMany(x => GetPotential(formation, unit, x, opposingSide))
                .ToList();
        }

        private IEnumerable<BattleAttack> GetPotential(
            IFormation formation, UnitGrouping unit, Count<Weapon> weapon, BattleSide opposingSide)
        {
            foreach (var f in opposingSide.Formations)
            {
                foreach (var group in f.Composition)
                {
                    yield return BattleAttack.Create(unit, formation, group, f, weapon);
                }
            }
        }
    }
}