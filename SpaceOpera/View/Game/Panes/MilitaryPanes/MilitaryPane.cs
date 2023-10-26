using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes;
using SpaceOpera.Core;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.MilitaryPanes
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

        private static readonly string s_FormationContainer = "military-pane-formation-container";
        private static readonly ActionRow<Type>.Style s_FormationHeaderStyle =
            new()
            {
                Container = "military-pane-formation-header"
            };
        private static readonly string s_FormationHeaderSpace = "military-pane-formation-header-space";
        private static readonly List<ActionRow<Type>.ActionConfiguration> s_FormationHeaderActions = new();

        private static readonly string s_FormationTable = "military-pane-formation-table";
        private static readonly ActionRow<IFormationDriver>.Style s_FormationRowStyle =
            new()
            {
                Container = "military-pane-formation-row",
                ActionContainer = "military-pane-formation-row-action-container"
            };
        private static readonly string s_Icon = "military-pane-formation-row-icon";
        private static readonly string s_Text = "military-pane-formation-row-text";
        private static readonly List<ActionRow<IFormationDriver>.ActionConfiguration> s_FormationActions = new();

        private World? _world;
        private Faction? _faction;
        private TabId _tab;

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public UiCompoundComponent Formations { get; }

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
                    uiElementFactory.GetClass(s_Body), new NoOpElementController());

            Formations =
                new ActionTable<IFormationDriver>(
                    _uiElementFactory.GetClass(s_FormationContainer),
                    ActionRow<Type>.Create(
                        typeof(IFormationDriver),
                        ActionId.Unknown,
                        uiElementFactory,
                        s_FormationHeaderStyle,
                        new List<IUiElement>()
                        {
                            new SimpleUiElement(
                                uiElementFactory.GetClass(s_FormationHeaderSpace), new InlayController())
                        },
                        s_FormationHeaderActions),
                    new DynamicKeyedTable<IFormationDriver, ActionRow<IFormationDriver>>(
                        uiElementFactory.GetClass(s_FormationTable),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        GetRange,
                        CreateRow,
                        Comparer<IFormationDriver>.Create((x, y) => x.Formation.Name.CompareTo(y.Formation.Name))));

            body.Add(Formations);
            SetBody(body);
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override object GetTab()
        {
            return _tab;
        }

        public override void SetTab(object id)
        {
            _tab = (TabId)id;
        }

        private ActionRow<IFormationDriver> CreateRow(IFormationDriver driver)
        {
            return ActionRow<IFormationDriver>.Create(
                driver,
                ActionId.Select,
                _uiElementFactory,
                s_FormationRowStyle,
                new List<IUiElement>()
                {
                    _iconFactory.Create(_uiElementFactory.GetClass(s_Icon), new InlayController(), driver),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_Text), new InlayController(), driver.Formation.Name)
                },
                s_FormationActions);
        }

        private IEnumerable<IFormationDriver> GetRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<IFormationDriver>();
            }
            return _tab switch
            {
                TabId.Army => _world.Formations.GetArmyDriversFor(_faction),
                TabId.Fleet => _world.Formations.GetFleetDriversFor(_faction),
                _ => Enumerable.Empty<IFormationDriver>(),
            };
        }
    }
}
