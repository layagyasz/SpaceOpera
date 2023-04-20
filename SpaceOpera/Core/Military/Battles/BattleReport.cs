using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Military.Battles
{
    public class BattleReport
    {
        public class FactionReport
        {
            public Faction Faction { get; }
            public BattleSideType Side { get; }
            public List<UnitReport> UnitReports { get; }

            public FactionReport(Faction faction, BattleSideType side, IEnumerable<UnitReport> unitReports)
            {
                Faction = faction;
                Side = side;
                UnitReports = unitReports.ToList();
            }

            public class Builder
            {
                private readonly Faction _faction;
                private readonly BattleSideType _side;
                private readonly Dictionary<Unit, UnitReport.Builder> _unitReports = new();

                public Builder(Faction faction, BattleSideType side)
                {
                    _faction = faction;
                    _side = side;
                }

                public UnitReport.Builder GetBuilderFor(Unit unit)
                {
                    if (!_unitReports.TryGetValue(unit, out var builder))
                    {
                        builder = new UnitReport.Builder(unit);
                        _unitReports.Add(unit, builder);
                    }
                    return builder;
                }

                public FactionReport Build()
                {
                    return new FactionReport(_faction, _side, _unitReports.Values.Select(x => x.Build()));
                }
            }
        }

        public class UnitReport
        {
            public Unit Unit { get; }

            public int Count { get; }
            public int Losses { get; }

            public float TotalOutputRawDamage { get; }
            public float TotalOutputOnTargetDamage { get; }
            public float TotalOutputHullDamage { get; }
            public float TotalOutputEffectiveDamage { get; }

            public float TotalInputRawDamage { get; }
            public float TotalInputOnTargetDamage { get; }
            public float TotalInputHullDamage { get; }
            public float TotalInputEffectiveDamage { get; }

            public UnitReport(
                Unit unit,
                int count,
                int losses,
                float totalOutputRawDamage,
                float totalOutputOnTargetDamage,
                float totalOutputHullDamage,
                float totalOutputEffectiveDamage,
                float totalInputRawDamage,
                float totalInputOnTargetDamage,
                float totalInputHullDamage, 
                float totalInputEffectiveDamage)
            {
                Unit = unit;
                Count = count;
                Losses = losses;
                TotalOutputRawDamage = totalOutputRawDamage;
                TotalOutputOnTargetDamage = totalOutputOnTargetDamage;
                TotalOutputHullDamage = totalOutputHullDamage;
                TotalOutputEffectiveDamage = totalOutputEffectiveDamage;
                TotalInputRawDamage = totalInputRawDamage;
                TotalInputOnTargetDamage = totalInputOnTargetDamage;
                TotalInputHullDamage = totalInputHullDamage;
                TotalInputEffectiveDamage = totalInputEffectiveDamage;
            }

            public float GetManeuverEfficiency()
            {
                return 1 - TotalInputOnTargetDamage / TotalInputRawDamage;
            }

            public float GetShieldEfficiency()
            {
                return 1 - TotalInputHullDamage / TotalInputOnTargetDamage;
            }

            public float GetArmorEfficiency()
            {
                return 1 - TotalInputEffectiveDamage / TotalInputHullDamage;
            }

            public float GetDefenseEfficiency()
            {
                return 1 - TotalInputRawDamage / TotalInputEffectiveDamage;
            }

            public float GetTargetingEfficiency()
            {
                return TotalOutputOnTargetDamage / TotalOutputRawDamage;
            }

            public float GetShieldPenetrationEfficiency()
            {
                return TotalOutputHullDamage / TotalOutputOnTargetDamage;
            }

            public float GetArmorPenetrationEfficiency()
            {
                return TotalOutputEffectiveDamage / TotalOutputHullDamage;
            }

            public float GetAttackEfficiency()
            {
                return TotalOutputEffectiveDamage / TotalOutputOnTargetDamage;
            }

            public float GetLossProportion()
            {
                return 1f * Losses / Count;
            }

            public class Builder
            {
                private readonly Unit _unit;

                private int _count;
                private int _losses;

                private float _totalOutputRawDamage;
                private float _totalOutputOnTargetDamage;
                private float _totalOutputHullDamage;
                private float _totalOutputEffectiveDamage;

                private float _totalInputRawDamage;
                private float _totalInputOnTargetDamage;
                private float _totalInputHullDamage;
                private float _totalInputEffectiveDamage;

                public Builder(Unit unit)
                {
                    _unit = unit;
                }

                public Builder Add(int count)
                {
                    _count += count;
                    return this;
                }

                public Builder AddLosses(int losses)
                {
                    _losses += losses;
                    return this;
                }

                public Builder AddOutputRawDamage(float damage)
                {
                    _totalOutputRawDamage += damage;
                    return this;
                }

                public Builder AddOutputOnTargetDamage(float damage)
                {
                    _totalOutputOnTargetDamage += damage;
                    return this;
                }

                public Builder AddOutputHullDamage(float damage)
                {
                    _totalOutputHullDamage += damage;
                    return this;
                }

                public Builder AddOutputEffectiveDamage(float damage)
                {
                    _totalOutputEffectiveDamage += damage;
                    return this;
                }

                public Builder AddInputRawDamage(float damage)
                {
                    _totalInputRawDamage += damage;
                    return this;
                }

                public Builder AddInputOnTargetDamage(float damage)
                {
                    _totalInputOnTargetDamage += damage;
                    return this;
                }

                public Builder AddInputHullDamage(float damage)
                {
                    _totalInputHullDamage += damage;
                    return this;
                }

                public Builder AddInputEffectiveDamage(float damage)
                {
                    _totalInputEffectiveDamage += damage;
                    return this;
                }

                public UnitReport Build()
                {
                    return new(
                        _unit,
                        _count,
                        _losses,
                        _totalOutputRawDamage, 
                        _totalOutputOnTargetDamage, 
                        _totalOutputHullDamage,
                        _totalOutputEffectiveDamage,
                        _totalInputRawDamage,
                        _totalInputOnTargetDamage, 
                        _totalInputHullDamage, 
                        _totalInputEffectiveDamage);
                }
            }
        }

        public class Builder
        {
            private readonly Dictionary<Faction, FactionReport.Builder> _factionBuilders = new();

            public FactionReport.Builder GetBuilderFor(BattleSideType side, Faction faction)
            {
                _factionBuilders.TryGetValue(faction, out var builder);
                if (builder == null)
                {
                    builder = new FactionReport.Builder(faction, side);
                    _factionBuilders.Add(faction, builder);
                }
                return builder;
            }

            public BattleReport Build()
            {
                return new BattleReport(_factionBuilders.Values.Select(x => x.Build()));
            }
        }

        public List<FactionReport> FactionReports { get; }

        public BattleReport(IEnumerable<FactionReport> factionReports)
        {
            FactionReports = factionReports.ToList();
        }
    }
}