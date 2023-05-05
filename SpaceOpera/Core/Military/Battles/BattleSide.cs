using Cardamom.Trackers;

namespace SpaceOpera.Core.Military.Battles
{
    public class BattleSide
    {
        private readonly BattleSideType _side;
        private readonly List<IAtomicFormation> _formations = new();

        public BattleSide(BattleSideType side)
        {
            _side = side;
        }

        public void Add(IAtomicFormation Formation)
        {
            _formations.Add(Formation);
        }

        public bool Contains(IAtomicFormation formation)
        {
            return _formations.Contains(formation);
        }

        public IEnumerable<IAtomicFormation> GetFormations()
        {
            return _formations;
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
                    if (group.Count.IsEmpty())
                    {
                        continue;
                    }

                    var abs = group.GetCurrentAbsorption();
                    Damage totalOutput = new();
                    Damage totalOnTarget = new();
                    float totalEffective = 0;
                    foreach (var attack in grouping)
                    {
                        var raw = attack.ComputeRaw();
                        var onTarget = attack.ComputeOnTarget();
                        var onTargetSum = onTarget.GetTotal();
                        var effective = attack.ComputeFinal(1 - abs).GetTotal();
                        totalOutput += raw;
                        totalOnTarget += onTarget;
                        totalEffective += effective;
                        report
                            .GetBuilderFor(_side, attack.Attack.AttackerFormation.Faction)
                            .GetBuilderFor(attack.Attack.Attacker.Unit)
                            .AddOutputRawDamage(raw.GetTotal())
                            .AddOutputOnTargetDamage(onTargetSum)
                            .AddOutputHullDamage((1 - abs) * onTargetSum)
                            .AddOutputEffectiveDamage(effective);
                    }
                    var losses = group.DamageHitpoints(totalEffective);
                    group.DamageShield(abs * totalOnTarget);
                    lostCommand += group.Unit.Command * losses;

                    report
                        .GetBuilderFor(ReverseSide(_side), formation.Key.Faction)
                        .GetBuilderFor(group.Unit)
                        .AddLosses(losses)
                        .AddInputRawDamage(totalOutput.GetTotal())
                        .AddInputOnTargetDamage(totalOnTarget.GetTotal())
                        .AddInputHullDamage((1 - abs) * totalOnTarget.GetTotal())
                        .AddInputEffectiveDamage(totalEffective);
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

        public void Remove(IAtomicFormation formation)
        {
            _formations.Remove(formation);
        }

        private static List<BattleAttack> GetPotential(
            IAtomicFormation formation, UnitGrouping unit, BattleSide opposingSide)
        {
            return unit.Unit.Weapons.GetCounts()
                .SelectMany(x => GetPotential(formation, unit, x, opposingSide))
                .ToList();
        }

        private static IEnumerable<BattleAttack> GetPotential(
            IAtomicFormation formation, UnitGrouping unit, Count<Weapon> weapon, BattleSide opposingSide)
        {
            foreach (var f in opposingSide._formations)
            {
                foreach (var group in f.Composition)
                {
                    yield return BattleAttack.Create(unit, formation, group, f, weapon);
                }
            }
        }

        private static BattleSideType ReverseSide(BattleSideType Side)
        {
            return Side switch
            {
                BattleSideType.Offense => BattleSideType.Defense,
                BattleSideType.Defense => BattleSideType.Offense,
                _ => BattleSideType.None,
            };
        }
    }
}