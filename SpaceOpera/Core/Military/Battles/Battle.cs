using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Battles
{
    public class Battle
    {
        public INavigable Location { get; }

        private readonly BattleReport.Builder _report = new();
        private readonly BattleSide _offense = new();
        private readonly BattleSide _defense = new();

        public Battle(INavigable location)
        {
            Location = location;
        }

        public void Add(IFormation formation, BattleSideType side)
        {
            switch (side)
            {
                case BattleSideType.Offense:
                    lock (_offense)
                    {
                        _offense.Add(formation);
                    }
                    break;
                case BattleSideType.Defense:
                    lock (_defense)
                    {
                        _defense.Add(formation);
                    }
                    break;
                default:
                    throw new ArgumentException("Unsupported BattleSideType");
            }

            var factionReport = _report.GetBuilderFor(formation.Faction);
            foreach (var group in formation.Composition)
            {
                factionReport.GetBuilderFor(group.Unit).Add(group.Count.Amount);
            }
        }

        public bool Contains(IFormation formation)
        {
            return _offense.Contains(formation) || _defense.Contains(formation);
        }

        public BattleSideType GetBattleSide(IFormation formation)
        {
            if (_offense.Contains(formation))
            {
                return BattleSideType.Offense;
            }
            if (_defense.Contains(formation))
            {
                return BattleSideType.Defense;
            }
            return BattleSideType.None;
        }

        public IEnumerable<IFormation> GetFormations(BattleSideType type)
        {
            return type switch
            {
                BattleSideType.Offense => _offense.GetFormations(),
                BattleSideType.Defense => _defense.GetFormations(),
                _ => throw new ArgumentException($"Unsupported BattleSideType: [{type}]."),
            };
        }

        public BattleReport GetReport()
        {
            return _report.Build();
        }

        public void Remove(IFormation formation, BattleSideType side)
        {
            switch (side)
            {
                case BattleSideType.Offense:
                    lock (_offense)
                    {
                        _offense.Remove(formation);
                    }
                    return;
                case BattleSideType.Defense:
                    lock (_defense)
                    {
                        _defense.Remove(formation);
                    }
                    return;
                default:
                    throw new ArgumentException($"Unsupported BattleSideType: [{side}].");
            }
        }

        public void Tick(Random random)
        {
            lock (_offense)
            {
                lock (_defense)
                {
                    var defenses = _defense.GetAttacks(_offense, random);
                    var attacks = _offense.GetAttacks(_defense, random);
                    BattleSide.Damage(attacks, _report);
                    BattleSide.Damage(defenses, _report);
                }
            }
        }
    }
}