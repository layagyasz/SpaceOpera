using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;
using SpaceOpera.Controller.Game.Panes;
using SpaceOpera.Controller.Game.Panes.MilitaryPanes;
using SpaceOpera.Core;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.View.Game.Panes.MilitaryPanes
{
    public class FormationsTab : DynamicUiCompoundComponent
    {
        private static readonly string s_Container = "military-pane-body";
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

        private static readonly string s_SummaryContainer = "military-pane-formation-summary";

        abstract class BaseComponentFactory<T> : IKeyedElementFactory<T>
        {
            protected readonly UiElementFactory _uiElementFactory;
            protected readonly IconFactory _iconFactory;

            protected World? _world;
            protected Faction? _faction;

            protected BaseComponentFactory(UiElementFactory uiElementFactory, IconFactory iconFactory)
            {
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
            }

            public void Populate(World? world, Faction? faction)
            {
                _world = world;
                _faction = faction;
            }

            public abstract IKeyedUiElement<T> Create(T driver);
        }

        class ArmyComponentFactory : BaseComponentFactory<ArmyDriver>
        {
            public ArmyComponentFactory(UiElementFactory uiElementFactory, IconFactory iconFactory)
                : base(uiElementFactory, iconFactory) { }

            public override IKeyedUiElement<ArmyDriver> Create(ArmyDriver driver)
            {
                return KeyedUiComponent<ArmyDriver>.Wrap(
                    driver, ArmySummaryComponent.Create(_world!, _faction!, driver, _uiElementFactory, _iconFactory));
            }
        }

        class FleetComponentFactory : BaseComponentFactory<FleetDriver>
        {
            public FleetComponentFactory(UiElementFactory uiElementFactory, IconFactory iconFactory)
                : base(uiElementFactory, iconFactory) { }

            public override IKeyedUiElement<FleetDriver> Create(FleetDriver driver)
            {
                return KeyedUiComponent<FleetDriver>.Wrap(
                    driver, FleetSummaryComponent.Create(driver, _uiElementFactory, _iconFactory));
            }
        }

        public ActionTable<IFormationDriver> Formations { get; }
        public UiCompoundComponent Summary { get; }

        private readonly DelegatedRange<IFormationDriver> _range;
        private readonly ArmyComponentFactory _armyComponentFactory;
        private readonly FleetComponentFactory _fleetComponentFactory;

        private FormationsTab(
            Class @class,
            ActionTable<IFormationDriver> formations,
            DynamicUiCompoundComponent summary,
            DelegatedRange<IFormationDriver> range,
            ArmyComponentFactory armyComponentFactory,
            FleetComponentFactory fleetComponentFactory)
            : base(
                  new FormationsTabController(),
                  new DynamicUiSerialContainer(
                      @class, new NoOpElementController(), UiSerialContainer.Orientation.Horizontal))
        {
            Formations = formations;
            Add(Formations);

            Summary = summary;
            Add(Summary);

            _range = range;
            _armyComponentFactory = armyComponentFactory;
            _fleetComponentFactory = fleetComponentFactory;
        }

        public void Populate(World? world, Faction? faction)
        {
            _armyComponentFactory.Populate(world, faction);
            _fleetComponentFactory.Populate(world, faction);
        }

        public void SetRange(KeyRange<IFormationDriver>? range)
        {
            _range.SetRange(range);
        }

        public void SetSummary(IFormationDriver? driver)
        {
            Summary.Clear(/* dispose= */ true);
            if (driver != null)
            {
                if (driver is ArmyDriver army)
                {
                    var component = _armyComponentFactory.Create(army);
                    component.Initialize();
                    Summary.Add(component);
                }
                else if (driver is FleetDriver fleet)
                {
                    var component = _fleetComponentFactory.Create(fleet);
                    component.Initialize();
                    Summary.Add(component);
                }
            }
        }

        public static FormationsTab Create(UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var range = new DelegatedRange<IFormationDriver>();
            return new(
                uiElementFactory.GetClass(s_Container),
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
                        range.GetRange,
                        new SimpleKeyedElementFactory<IFormationDriver>(uiElementFactory, iconFactory, CreateRow),
                        Comparer<IFormationDriver>.Create((x, y) => x.Formation.Name.CompareTo(y.Formation.Name))),
                    /* isSelectable=*/ true),
                new DynamicUiCompoundComponent(
                    new TabComponentController(), 
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_SummaryContainer),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical)),
                range,
                new ArmyComponentFactory(uiElementFactory, iconFactory),
                new FleetComponentFactory(uiElementFactory, iconFactory));
        }

        private static IKeyedUiElement<IFormationDriver> CreateRow(
            IFormationDriver driver, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return ActionRow<IFormationDriver>.Create(
                driver,
                ActionId.Select,
                ActionId.Unknown,
                uiElementFactory,
                s_FormationRowStyle,
                new List<IUiElement>()
                {
                    iconFactory.Create(uiElementFactory.GetClass(s_Icon), new InlayController(), driver),
                    new TextUiElement(
                        uiElementFactory.GetClass(s_Text), new InlayController(), driver.Formation.Name)
                },
                s_FormationActions);
        }
    }
}
