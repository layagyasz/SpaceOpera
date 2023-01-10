using SpaceOpera.Core.Politics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military.Battles
{
    class BattleReport
    {
        public class FactionReport
        {
            public Faction Faction { get; }
            public List<UnitReport> UnitReports { get; }

            public FactionReport(Faction Faction, IEnumerable<UnitReport> UnitReports)
            {
                this.Faction = Faction;
                this.UnitReports = UnitReports.ToList();
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
                private readonly Faction _Faction;
                private readonly Dictionary<Unit, UnitReport.Builder> _UnitReports = 
                    new Dictionary<Unit, UnitReport.Builder>();

                public Builder(Faction Faction)
                {
                    _Faction = Faction;
                }

                public UnitReport.Builder GetBuilderFor(Unit Unit)
                {
                    _UnitReports.TryGetValue(Unit, out UnitReport.Builder builder);
                    if (builder == null)
                    {
                        builder = new UnitReport.Builder(Unit);
                        _UnitReports.Add(Unit, builder);
                    }
                    return builder;
                }

                public FactionReport Build()
                {
                    return new FactionReport(_Faction, _UnitReports.Values.Select(x => x.Build()));
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
                Unit Unit,
                int Count,
                int Losses,
                float TotalRawDamage, 
                float TotalOnTargetDamage,
                float TotalEffectiveDamage,
                float TotalReceivedDamage,
                float TotalHullDamage, 
                float TotalTakenDamage)
            {
                this.Unit = Unit;
                this.Count = Count;
                this.Losses = Losses;
                this.TotalRawDamage = TotalRawDamage;
                this.TotalOnTargetDamage = TotalOnTargetDamage;
                this.TotalEffectiveDamage = TotalEffectiveDamage;
                this.TotalReceivedDamage = TotalReceivedDamage;
                this.TotalHullDamage = TotalHullDamage;
                this.TotalTakenDamage = TotalTakenDamage;
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
                private readonly Unit _Unit;

                private int _Count;
                private int _Losses;

                private float _TotalRawDamage;
                private float _TotalOnTargetDamage;
                private float _TotalEffectiveDamage;

                private float _TotalReceivedDamage;
                private float _TotalHullDamage;
                private float _TotalTakenDamage;

                public Builder(Unit Unit)
                {
                    _Unit = Unit;
                }

                public Builder Add(int Count)
                {
                    _Count += Count;
                    return this;
                }

                public Builder AddLosses(int Losses)
                {
                    _Losses += Losses;
                    return this;
                }

                public Builder AddRawDamage(float Damage)
                {
                    _TotalRawDamage += Damage;
                    return this;
                }

                public Builder AddOnTargetDamage(float Damage)
                {
                    _TotalOnTargetDamage += Damage;
                    return this;
                }

                public Builder AddEffectiveDamage(float Damage)
                {
                    _TotalEffectiveDamage += Damage;
                    return this;
                }

                public Builder AddReceivedDamage(float Damage)
                {
                    _TotalReceivedDamage += Damage;
                    return this;
                }

                public Builder AddHullDamage(float Damage)
                {
                    _TotalHullDamage += Damage;
                    return this;
                }

                public Builder AddTakenDamage(float Damage)
                {
                    _TotalTakenDamage += Damage;
                    return this;
                }

                public UnitReport Build()
                {
                    return new UnitReport(
                        _Unit,
                        _Count,
                        _Losses,
                        _TotalRawDamage, 
                        _TotalOnTargetDamage, 
                        _TotalEffectiveDamage,
                        _TotalReceivedDamage, 
                        _TotalHullDamage, 
                        _TotalTakenDamage);
                }
            }
        }

        public class Builder
        {
            private readonly Dictionary<Faction, FactionReport.Builder> _FactionBuilders = 
                new Dictionary<Faction, FactionReport.Builder>();

            public FactionReport.Builder GetBuilderFor(Faction Faction)
            {
                _FactionBuilders.TryGetValue(Faction, out FactionReport.Builder builder);
                if (builder == null)
                {
                    builder = new FactionReport.Builder(Faction);
                    _FactionBuilders.Add(Faction, builder);
                }
                return builder;
            }

            public BattleReport Build()
            {
                return new BattleReport(_FactionBuilders.Values.Select(x => x.Build()));
            }
        }

        public List<FactionReport> FactionReports { get; }

        public BattleReport(IEnumerable<FactionReport> FactionReports)
        {
            this.FactionReports = FactionReports.ToList();
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