namespace SpaceOpera.Core.Military.Battles
{
    public class Battle
    {
        private readonly BattleReport.Builder _report = new();
        private readonly BattleSide _offense = new();
        private readonly BattleSide _defense = new();

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
                factionReport.GetBuilderFor(group.Unit).Add(group.Count);
            }
        }

        public BattleSideType GetBattleSide(IFormation formation)
        {
            if (_offense.Formations.Contains(formation))
            {
                return BattleSideType.Offense;
            }
            if (_defense.Formations.Contains(formation))
            {
                return BattleSideType.Defense;
            }
            return BattleSideType.None;
        }

        public IEnumerable<IFormation> GetFormations(BattleSideType type)
        {
            switch (type)
            {
                case BattleSideType.Offense:
                    return _offense.Formations;
                case BattleSideType.Defense:
                    return _defense.Formations;
                default:
                    throw new ArgumentException();
            }
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
                    throw new ArgumentException();
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
                    _defense.Damage(attacks, _report);
                    _offense.Damage(defenses, _report);
                }
            }
        }
    }
}