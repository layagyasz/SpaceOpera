using Cardamom.Trackers;

namespace SpaceOpera.Core.Military.Battles
{
    public class BattleSide
    {
        private List<IFormation> _formations = new();

        public void Add(IFormation Formation)
        {
            _formations.Add(Formation);
        }

        public bool Contains(IFormation formation)
        {
            return _formations.Contains(formation);
        }

        public IEnumerable<IFormation> GetFormations()
        {
            return _formations;
        }

        public static void Damage(List<DistributedBattleAttack> attacks, BattleReport.Builder report)
        {
            foreach (var formation in attacks.GroupBy(x => x.Attack.TargetFormation))
            {
                float currentCommand = formation.Key.GetCommand();
                float lostCommand = 0;
                foreach (var grouping in formation.GroupBy(x => x.Attack.Target))
                {
                    var group = grouping.Key;
                    if (group.Count.IsEmpty())
                    {
                        continue;
                    }

                    var abs = group.GetCurrentAbsorption();
                    Damage totalOnTarget = new();
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
                formation.Key.Cohesion.Change(-10 * lostCommand / currentCommand);
            }
        }

        public List<DistributedBattleAttack> GetAttacks(BattleSide opposingSide, Random random)
        {
            var result = new List<DistributedBattleAttack>();
            foreach (var formation in _formations)
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
            _formations.Remove(formation);
        }

        private static List<BattleAttack> GetPotential(IFormation formation, UnitGrouping unit, BattleSide opposingSide)
        {
            return unit.Unit.Weapons.GetCounts()
                .SelectMany(x => GetPotential(formation, unit, x, opposingSide))
                .ToList();
        }

        private static IEnumerable<BattleAttack> GetPotential(
            IFormation formation, UnitGrouping unit, Count<Weapon> weapon, BattleSide opposingSide)
        {
            foreach (var f in opposingSide._formations)
            {
                foreach (var group in f.Composition)
                {
                    yield return BattleAttack.Create(unit, formation, group, f, weapon);
                }
            }
        }
    }
}