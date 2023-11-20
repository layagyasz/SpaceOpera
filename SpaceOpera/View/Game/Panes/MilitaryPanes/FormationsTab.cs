using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;
using Cardamom.Ui.Controller;

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

        public ActionTable<IFormationDriver> Formations { get; }

        private readonly DelegatedRange<IFormationDriver> _range;

        private FormationsTab(
            Class @class, ActionTable<IFormationDriver> formations, DelegatedRange<IFormationDriver> range)
            : base(
                  new NoOpController(), 
                  new DynamicUiSerialContainer(
                      @class, new NoOpElementController(), UiSerialContainer.Orientation.Horizontal))
        {
            Formations = formations;
            _range = range;

            Add(Formations);
        }

        public void SetRange(KeyRange<IFormationDriver>? range)
        {
            _range.SetRange(range);
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
                        new FormationComponentFactory(uiElementFactory, iconFactory),
                        Comparer<IFormationDriver>.Create((x, y) => x.Formation.Name.CompareTo(y.Formation.Name)))),
                range);
        }
    }
}
