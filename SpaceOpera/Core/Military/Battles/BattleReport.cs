using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Military.Battles
{
    public class BattleReport
    {
        public class FactionReport
        {
            public Faction Faction { get; }
            public List<UnitReport> UnitReports { get; }

            public FactionReport(Faction faction, IEnumerable<UnitReport> unitReports)
            {
                this.Faction = faction;
                this.UnitReports = unitReports.ToList();
            }

            public void Print()
            {
                Console.WriteLine("\t" + Faction.Name);
                foreach (var unitReport in UnitReports)
                {
                    unitReport.Print();
                }
            }

            public class Builder
            {
                private readonly Faction _faction;
                private readonly Dictionary<Unit, UnitReport.Builder> _unitReports = new();

                public Builder(Faction faction)
                {
                    _faction = faction;
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
                    return new FactionReport(_faction, _unitReports.Values.Select(x => x.Build()));
                }
            }
        }

        public class UnitReport
        {
            public Unit Unit { get; }

            public int Count { get; }
            public int Losses { get; }

            public float TotalRawDamage { get; }
            public float TotalOnTargetDamage { get; }
            public float TotalEffectiveDamage { get; }

            public float TotalReceivedDamage { get; }
            public float TotalHullDamage { get; }
            public float TotalTakenDamage { get; }

            public UnitReport(
                Unit unit,
                int count,
                int losses,
                float totalRawDamage, 
                float totalOnTargetDamage,
                float totalEffectiveDamage,
                float totalReceivedDamage,
                float totalHullDamage, 
                float totalTakenDamage)
            {
                Unit = unit;
                Count = count;
                Losses = losses;
                TotalRawDamage = totalRawDamage;
                TotalOnTargetDamage = totalOnTargetDamage;
                TotalEffectiveDamage = totalEffectiveDamage;
                TotalReceivedDamage = totalReceivedDamage;
                TotalHullDamage = totalHullDamage;
                TotalTakenDamage = totalTakenDamage;
            }

            public float GetArmorEfficiency()
            {
                return 1 - TotalTakenDamage / TotalHullDamage;
            }

            public float GetDamageEfficiency()
            {
                return TotalEffectiveDamage / TotalOnTargetDamage;
            }

            public float GetLossProportion()
            {
                return 1f * Losses / Count;
            }

            public float GetTargetingEfficiency()
            {
                return TotalOnTargetDamage / TotalRawDamage;
            }

            public float GetShieldEfficiency()
            {
                return 1 - TotalHullDamage / TotalReceivedDamage;
            }

            public void Print()
            {
                Console.WriteLine("\t\t" + Unit.Name);
                Console.WriteLine("\t\t\tLosses: {0}/{1} ({2})", Losses, Count, GetLossProportion());
                Console.WriteLine(
                    "\t\t\tDamage Out: {0} -> {1} ({2}) -> {3} ({4})", 
                    TotalRawDamage, 
                    TotalOnTargetDamage,
                    GetTargetingEfficiency(), 
                    TotalEffectiveDamage,
                    GetDamageEfficiency());
                Console.WriteLine(
                    "\t\t\tDamage In: {0} -> {1} ({2}) -> {3} ({4})", 
                    TotalReceivedDamage,
                    TotalHullDamage, 
                    GetShieldEfficiency(),
                    TotalTakenDamage,
                    GetArmorEfficiency());
            }

            public class Builder
            {
                private readonly Unit _unit;

                private int _count;
                private int _losses;

                private float _totalRawDamage;
                private float _totalOnTargetDamage;
                private float _totalEffectiveDamage;

                private float _totalReceivedDamage;
                private float _totalHullDamage;
                private float _totalTakenDamage;

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

                public Builder AddRawDamage(float damage)
                {
                    _totalRawDamage += damage;
                    return this;
                }

                public Builder AddOnTargetDamage(float damage)
                {
                    _totalOnTargetDamage += damage;
                    return this;
                }

                public Builder AddEffectiveDamage(float damage)
                {
                    _totalEffectiveDamage += damage;
                    return this;
                }

                public Builder AddReceivedDamage(float damage)
                {
                    _totalReceivedDamage += damage;
                    return this;
                }

                public Builder AddHullDamage(float damage)
                {
                    _totalHullDamage += damage;
                    return this;
                }

                public Builder AddTakenDamage(float damage)
                {
                    _totalTakenDamage += damage;
                    return this;
                }

                public UnitReport Build()
                {
                    return new UnitReport(
                        _unit,
                        _count,
                        _losses,
                        _totalRawDamage, 
                        _totalOnTargetDamage, 
                        _totalEffectiveDamage,
                        _totalReceivedDamage, 
                        _totalHullDamage, 
                        _totalTakenDamage);
                }
            }
        }

        public class Builder
        {
            private readonly Dictionary<Faction, FactionReport.Builder> _FactionBuilders = new();

            public FactionReport.Builder GetBuilderFor(Faction faction)
            {
                _FactionBuilders.TryGetValue(faction, out var builder);
                if (builder == null)
                {
                    builder = new FactionReport.Builder(faction);
                    _FactionBuilders.Add(faction, builder);
                }
                return builder;
            }

            public BattleReport Build()
            {
                return new BattleReport(_FactionBuilders.Values.Select(x => x.Build()));
            }
        }

        public List<FactionReport> FactionReports { get; }

        public BattleReport(IEnumerable<FactionReport> factionReports)
        {
            FactionReports = factionReports.ToList();
        }

        public void Print()
        {
            Console.WriteLine("[Battle Report]");
            foreach (var factionReport in FactionReports)
            {
                factionReport.Print();
            }
        }
    }
}