using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Controller.Panes;
using SpaceOpera.Core.Military.Battles;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Components;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.View.Panes.BattlePanes
{
    public class BattlePane : SimpleGamePane
    {
        private static readonly string s_ClassName = "battle-pane";
        private static readonly string s_Title = "battle-pane-title";
        private static readonly string s_Close = "battle-pane-close";
        private static readonly string s_Body = "battle-pane-body";
        private static readonly string s_Side = "battle-pane-side";
        private static readonly string s_SideHeader = "battle-pane-side-header";
        private static readonly string s_SideFactionTable = "battle-pane-side-faction-table";

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;
        private readonly Configuration _configuration = new();

        private Battle? _battle;

        class Configuration
        {
            private BattleReport? _report;

            public void SetReport(BattleReport? report)
            {
                _report = report;
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

        public BattlePane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new GamePaneController(),
                  uiElementFactory.GetClass(s_ClassName),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), string.Empty),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1)
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            var offenseTable =
                new DynamicKeyedTable<Faction, FactionComponent>(
                    uiElementFactory.GetClass(s_SideFactionTable),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical,
                    _configuration.GetOffense,
                    CreateRow,
                    Comparer<Faction>.Create((x, y) => x.Name.CompareTo(y.Name)));
            var wrappedOffenseTable =
                new DynamicUiSerialContainer(
                    _uiElementFactory.GetClass(s_Side),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    new TextUiElement(_uiElementFactory.GetClass(s_SideHeader), new ButtonController(), "Attackers"),
                    offenseTable
                };

            var defenseTable =
                new DynamicKeyedTable<Faction, FactionComponent>(
                    uiElementFactory.GetClass(s_SideFactionTable),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical,
                    _configuration.GetDefense,
                    CreateRow,
                    Comparer<Faction>.Create((x, y) => x.Name.CompareTo(y.Name)));
            var wrappedDefenseTable =
                new DynamicUiSerialContainer(
                    _uiElementFactory.GetClass(s_Side),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    new TextUiElement(_uiElementFactory.GetClass(s_SideHeader), new ButtonController(), "Defenders"),
                    defenseTable
                };

            var body = new
                DynamicUiSerialContainer(uiElementFactory.GetClass(s_Body),
                new NoOpElementController<UiSerialContainer>(),
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
            _configuration.SetReport(_battle?.GetReport());
            base.Refresh();
        }

        private FactionComponent CreateRow(Faction faction)
        {
            return new FactionComponent(faction, _uiElementFactory, _iconFactory);
        }
    }
}
