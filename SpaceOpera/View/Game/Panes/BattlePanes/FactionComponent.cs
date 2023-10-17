using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;
using SpaceOpera.Core.Military;

namespace SpaceOpera.View.Game.Panes.BattlePanes
{
    public class FactionComponent : DynamicUiSerialContainer, IKeyedUiElement<Faction>
    {
        private static readonly string s_Container = "battle-pane-faction-container";
        private static readonly string s_HeaderContainer = "battle-pane-faction-header";
        private static readonly string s_HeaderIcon = "battle-pane-faction-header-icon";
        private static readonly string s_HeaderText = "battle-pane-faction-header-text";
        private static readonly string s_UnitTable = "battle-pane-faction-unit-table";

        public Faction Key { get; }

        private readonly ReportWrapper _report;
        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public FactionComponent(
            Faction faction, ReportWrapper report, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                uiElementFactory.GetClass(s_Container),
                new NoOpElementController(),
                Orientation.Vertical)
        {
            Key = faction;
            _report = report;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            var header =
                new UiSerialContainer(
                    _uiElementFactory.GetClass(s_HeaderContainer), new ButtonController(), Orientation.Horizontal)
                {
                    _iconFactory.Create(_uiElementFactory.GetClass(s_HeaderIcon), new InlayController(), faction),
                    new TextUiElement(_uiElementFactory.GetClass(s_HeaderText), new InlayController(), faction.Name)
                };
            Add(header);

            var units =
                new DynamicKeyedTable<Unit, UnitComponent>(
                    _uiElementFactory.GetClass(s_UnitTable),
                    new NoOpElementController(),
                    Orientation.Vertical,
                    GetRange,
                    CreateRow,
                    Comparer<Unit>.Create((x, y) => x.Name.CompareTo(y.Name)));
            Add(units);
        }

        private IEnumerable<Unit> GetRange()
        {
            return _report.Get(Key).UnitReports.Select(x => x.Unit);
        }

        private UnitComponent CreateRow(Unit unit)
        {
            return new UnitComponent(Key, unit, _report, _uiElementFactory, _iconFactory);
        }
    }
}
