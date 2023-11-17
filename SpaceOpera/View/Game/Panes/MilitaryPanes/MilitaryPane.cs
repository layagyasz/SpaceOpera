using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes;
using SpaceOpera.Core;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.MilitaryPanes
{
    public class MilitaryPane : MultiTabGamePane
    {
        private static readonly string s_Container = "military-pane";
        private static readonly string s_Title = "military-pane-title";
        private static readonly string s_Close = "military-pane-close";
        private static readonly string s_TabContainer = "military-pane-tab-container";
        private static readonly string s_TabOption = "military-pane-tab-option";
        private static readonly string s_Body = "military-pane-body";

        private static readonly string s_FormationContainer = "military-pane-formation-container";
        private static readonly ActionRowStyles.Style s_FormationHeaderStyle =
            new()
            {
                Container = "military-pane-formation-header"
            };
        private static readonly string s_FormationHeaderSpace = "military-pane-formation-header-space";
        private static readonly List<ActionRowStyles.ActionConfiguration> s_FormationHeaderActions = new();

        private static readonly string s_FormationTable = "military-pane-formation-table";
        private static readonly ActionRowStyles.Style s_FormationRowStyle =
            new()
            {
                Container = "military-pane-formation-row",
                ActionContainer = "military-pane-formation-row-action-container"
            };
        private static readonly string s_Icon = "military-pane-formation-row-icon";
        private static readonly string s_Text = "military-pane-formation-row-text";
        private static readonly List<ActionRowStyles.ActionConfiguration> s_FormationActions = new();

        public enum TabId
        {
            Army,
            Fleet
        }

        class FormationRange
        {
            public World? World { get; set; }
            public Faction? Faction { get; set; }
            public TabId Tab { get; set; }

            public IEnumerable<IFormationDriver> GetRange()
            {
                if (World == null || Faction == null)
                {
                    return Enumerable.Empty<IFormationDriver>();
                }
                return Tab switch
                {
                    TabId.Army => World.Formations.GetArmyDriversFor(Faction),
                    TabId.Fleet => World.Formations.GetFleetDriversFor(Faction),
                    _ => Enumerable.Empty<IFormationDriver>(),
                };
            }
        }

        class FormationComponentFactory : IKeyedElementFactory<IFormationDriver>
        {
            private readonly UiElementFactory _uiElementFactory;
            private readonly IconFactory _iconFactory;

            public FormationComponentFactory(UiElementFactory uiElementFactory, IconFactory iconFactory)
            {
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
            }

            public IKeyedUiElement<IFormationDriver> Create(IFormationDriver driver)
            {
                return ActionRow<IFormationDriver>.Create(
                    driver,
                    ActionId.Select,
                    ActionId.Unknown,
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
        }

        public UiCompoundComponent Formations { get; }

        private readonly FormationRange _range = new();

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
            var body = new
                DynamicUiContainer(
                    uiElementFactory.GetClass(s_Body), new NoOpElementController());

            Formations =
                new ActionTable<IFormationDriver>(
                    uiElementFactory.GetClass(s_FormationContainer),
                    ActionRow<Type>.Create(
                        typeof(IFormationDriver),
                        ActionId.Unknown,
                        ActionId.Unknown,
                        uiElementFactory,
                        s_FormationHeaderStyle,
                        new List<IUiElement>()
                        {
                            new SimpleUiElement(
                                uiElementFactory.GetClass(s_FormationHeaderSpace), new InlayController())
                        },
                        s_FormationHeaderActions),
                    DynamicKeyedContainer<IFormationDriver>.CreateSerial(
                        uiElementFactory.GetClass(s_FormationTable),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        _range.GetRange,
                        new FormationComponentFactory(uiElementFactory, iconFactory),
                        Comparer<IFormationDriver>.Create((x, y) => x.Formation.Name.CompareTo(y.Formation.Name))));

            body.Add(Formations);
            SetBody(body);
        }

        public override void Populate(params object?[] args)
        {
            _range.World = args[0] as World;
            _range.Faction = args[1] as Faction;
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override object GetTab()
        {
            return _range.Tab;
        }

        public override void SetTab(object id)
        {
            _range.Tab = (TabId)id;
        }
    }
}
