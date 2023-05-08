using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Panes;
using SpaceOpera.Core;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.MilitaryPanes
{
    public class MilitaryPane : MultiTabGamePane
    {
        public enum TabId
        {
            Army,
            Fleet
        }

        private static readonly string s_Container = "military-pane";
        private static readonly string s_Title = "military-pane-title";
        private static readonly string s_Close = "military-pane-close";
        private static readonly string s_TabContainer = "military-pane-tab-container";
        private static readonly string s_TabOption = "military-pane-tab-option";
        private static readonly string s_Body = "military-pane-body";
        private static readonly string s_MilitaryTable = "military-pane-military-table";

        private static readonly ActionRow<AtomicFormationDriver>.Style s_FormationRowStyle =
            new()
            {
                Container = "military-pane-formation-row"
            };
        private static readonly string s_Icon = "military-pane-formation-row-icon";
        private static readonly string s_Text = "military-pane-formation-row-text";

        private World? _world;
        private Faction? _faction;
        private TabId _tab;

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private readonly UiSerialContainer _formationTable;

        public MilitaryPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new MultiTabGamePaneController(),
                  uiElementFactory.GetClass(s_Container), 
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), "Military"),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1,
                  TabBar<TabId>.Create(
                    new List<TabBar<TabId>.Definition>()
                    {
                        new(TabId.Army, "Army"),
                        new(TabId.Fleet, "Fleet")
                    },
                    uiElementFactory.GetClass(s_TabContainer),
                    uiElementFactory.GetClass(s_TabOption)))
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
            var body = new
                DynamicUiContainer(
                    uiElementFactory.GetClass(s_Body), new NoOpElementController<UiContainer>());
            _formationTable =
                new DynamicKeyedTable<AtomicFormationDriver, ActionRow<AtomicFormationDriver>>(
                    uiElementFactory.GetClass(s_MilitaryTable),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical,
                    GetRange,
                    CreateRow,
                    Comparer<AtomicFormationDriver>.Create((x, y) => x.AtomicFormation.Name.CompareTo(y.AtomicFormation.Name)));
            body.Add(_formationTable);
            SetBody(body);
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override void SetTab(object id)
        {
            _tab = (TabId)id;
            _formationTable.SetOffset(0);
        }

        private ActionRow<AtomicFormationDriver> CreateRow(AtomicFormationDriver driver)
        {
            return ActionRow<AtomicFormationDriver>.Create(
                driver,
                ActionId.Select,
                _uiElementFactory,
                s_FormationRowStyle,
                new List<IUiElement>()
                {
                    _iconFactory.Create(_uiElementFactory.GetClass(s_Icon), new InlayController(), driver),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_Text), new InlayController(), driver.AtomicFormation.Name)
                },
                Enumerable.Empty<ActionRow<AtomicFormationDriver>.ActionConfiguration>());
        }

        private IEnumerable<AtomicFormationDriver> GetRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<AtomicFormationDriver>();
            }
            return _tab switch
            {
                TabId.Army => _world.FormationManager.GetDivisionDriversFor(_faction),
                TabId.Fleet => _world.FormationManager.GetFleetDriversFor(_faction),
                _ => Enumerable.Empty<AtomicFormationDriver>(),
            };
        }
    }
}
