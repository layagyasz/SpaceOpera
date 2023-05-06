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
        private static readonly string s_Container = "battle-pane";
        private static readonly string s_Title = "battle-pane-title";
        private static readonly string s_Close = "battle-pane-close";
        private static readonly string s_Body = "battle-pane-body";
        private static readonly string s_Side = "battle-pane-side";
        private static readonly string s_SideHeader = "battle-pane-side-header";
        private static readonly string s_SideFactionTable = "battle-pane-side-faction-table";

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;
        private readonly ReportWrapper _report = new();

        private Battle? _battle;

        public BattlePane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new GamePaneController(),
                  uiElementFactory.GetClass(s_Container),
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
                    _report.GetOffense,
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
                    _report.GetDefense,
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
            _report.SetReport(_battle?.GetReport());
            base.Refresh();
        }

        private FactionComponent CreateRow(Faction faction)
        {
            return new FactionComponent(faction, _report, _uiElementFactory, _iconFactory);
        }
    }
}
