using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Battles;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.View.Panes.BattlePanes
{
    public class ReportWrapper
    {
        private BattleReport? _report;

        public void SetReport(BattleReport? report)
        {
            _report = report;
        }

        public BattleReport.FactionReport Get(Faction faction)
        {
            return _report!.FactionReports.First(x => x.Faction == faction);
        }

        public BattleReport.UnitReport Get(Faction faction, Unit unit)
        {
            return Get(faction).UnitReports.First(x => x.Unit == unit);
        }

        public IEnumerable<Faction> GetOffense()
        {
            return GetSide(BattleSideType.Offense);
        }

        public IEnumerable<Faction> GetDefense()
        {
            return GetSide(BattleSideType.Defense);
        }

        private IEnumerable<Faction> GetSide(BattleSideType side)
        {
            return _report?.FactionReports.Where(x => x.Side == side).Select(x => x.Faction)
                ?? Enumerable.Empty<Faction>();
        }
    }
}
