using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Controller.Game.Panes;
using SpaceOpera.Core.Military.Battles;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.Game.Panes.BattlePanes
{
    public class BattlePane : SimpleGamePane
    {
        private static readonly string s_Container = "battle-pane";
        private static readonly string s_Title = "battle-pane-title";
        private static readonly string s_Close = "battle-pane-close";
        private static readonly string s_Body = "battle-pane-body";
        private static readonly string s_Side = "battle-pane-side";
        private static readonly string s_SideHeader = "battle-pane-side-header";
        private static readonly string s_SideFactionTable = "battle-pane-side-faction-table";

        class FactionRange : IRange<Faction>
        {
            private readonly bool _isOffense;
            private readonly ReportWrapper _report;

            public FactionRange(bool isOffense, ReportWrapper report)
            {
                _isOffense = isOffense;
                _report = report;
            }

            public IEnumerable<Faction> GetRange()
            {
                if (_report.Report == null)
                {
                    return Enumerable.Empty<Faction>();
                }
                return _isOffense ? _report.Report.GetOffense() : _report.Report.GetDefense();
            }
        }

        class FactionComponentFactory : IKeyedElementFactory<Faction>
        {
            private readonly UiElementFactory _uiElementFactory;
            private readonly IconFactory _iconFactory;
            private readonly ReportWrapper _report;

            public FactionComponentFactory(
                UiElementFactory uiElementFactory, IconFactory iconFactory, ReportWrapper report)
            {
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
                _report = report;
            }

            public IKeyedUiElement<Faction> Create(Faction faction)
            {
                return new FactionComponent(faction, _report, _uiElementFactory, _iconFactory);
            }
        }

        private readonly ReportWrapper _report = new();

        private Battle? _battle;

        public BattlePane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new GamePaneController(),
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), string.Empty),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1)
        {
            var componentFactory = new FactionComponentFactory(uiElementFactory, iconFactory, _report);
            var offenseTable =
                new DynamicKeyedTable<Faction>(
                    uiElementFactory.GetClass(s_SideFactionTable),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical,
                    new FactionRange(/* isOffense= */ true, _report),
                    componentFactory,
                    Comparer<Faction>.Create((x, y) => x.Name.CompareTo(y.Name)));
            var wrappedOffenseTable =
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Side),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    new TextUiElement(uiElementFactory.GetClass(s_SideHeader), new ButtonController(), "Attackers"),
                    offenseTable
                };

            var defenseTable =
                new DynamicKeyedTable<Faction>(
                    uiElementFactory.GetClass(s_SideFactionTable),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical,
                    new FactionRange(/* isOffense= */ false, _report),
                    componentFactory,
                    Comparer<Faction>.Create((x, y) => x.Name.CompareTo(y.Name)));
            var wrappedDefenseTable =
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Side),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    new TextUiElement(uiElementFactory.GetClass(s_SideHeader), new ButtonController(), "Defenders"),
                    defenseTable
                };

            var body = new
                DynamicUiSerialContainer(uiElementFactory.GetClass(s_Body),
                new NoOpElementController(),
                UiSerialContainer.Orientation.Horizontal)
            {
                wrappedOffenseTable,
                wrappedDefenseTable
            };
            SetBody(body);
        }

        public override void Populate(params object?[] args)
        {
            _battle = args[0] as Battle;
            SetTitle($"Battle in {_battle?.Location?.Name}");
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override void Refresh()
        {
            _report.Report = _battle?.GetReport();
            base.Refresh();
        }
    }
}
