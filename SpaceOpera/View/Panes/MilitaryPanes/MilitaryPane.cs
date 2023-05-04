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

        private static readonly string s_Title = "Military";

        private static readonly string s_ClassName = "military-pane";
        private static readonly string s_TitleClassName = "military-pane-title";
        private static readonly string s_CloseClass = "military-pane-close";
        private static readonly string s_TabContainerClassName = "military-pane-tab-container";
        private static readonly string s_TabOptionClassName = "military-pane-tab-option";
        private static readonly string s_BodyClassName = "military-pane-body";
        private static readonly string s_MilitaryTableClassName = "military-pane-military-table";

        private static readonly ActionRow<FormationDriver>.Style s_FormationRowStyle =
            new()
            {
                Container = "military-pane-formation-row"
            };
        private static readonly string s_IconClassName = "military-pane-formation-row-icon";
        private static readonly string s_TextClassName = "military-pane-formation-row-text";

        private World? _world;
        private Faction? _faction;
        private TabId _tab;

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private readonly UiSerialContainer _formationTable;

        public MilitaryPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new MultiTabGamePaneController(),
                  uiElementFactory.GetClass(s_ClassName), 
                  new TextUiElement(uiElementFactory.GetClass(s_TitleClassName), new ButtonController(), s_Title),
                  uiElementFactory.CreateSimpleButton(s_CloseClass).Item1,
                  TabBar<TabId>.Create(
                    new List<TabBar<TabId>.Definition>()
                    {
                        new(TabId.Army, "Army"),
                        new(TabId.Fleet, "Fleet")
                    },
                    uiElementFactory.GetClass(s_TabContainerClassName),
                    uiElementFactory.GetClass(s_TabOptionClassName)))
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
            var body = new
                DynamicUiContainer(
                    uiElementFactory.GetClass(s_BodyClassName), new NoOpElementController<UiContainer>());
            _formationTable =
                new DynamicKeyedTable<FormationDriver, ActionRow<FormationDriver>>(
                    uiElementFactory.GetClass(s_MilitaryTableClassName),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical,
                    GetRange,
                    CreateRow,
                    Comparer<FormationDriver>.Create((x, y) => x.Formation.Name.CompareTo(y.Formation.Name)));
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

        private ActionRow<FormationDriver> CreateRow(FormationDriver driver)
        {
            return ActionRow<FormationDriver>.Create(
                driver,
                ActionId.Select,
                _uiElementFactory,
                s_FormationRowStyle,
                new List<IUiElement>()
                {
                    _iconFactory.Create(_uiElementFactory.GetClass(s_IconClassName), new InlayController(), driver),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_TextClassName), new InlayController(), driver.Formation.Name)
                },
                Enumerable.Empty<ActionRow<FormationDriver>.ActionConfiguration>());
        }

        private IEnumerable<FormationDriver> GetRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<FormationDriver>();
            }
            return _tab switch
            {
                TabId.Army => _world.FormationManager.GetDivisionDriversFor(_faction),
                TabId.Fleet => _world.FormationManager.GetFleetDriversFor(_faction),
                _ => Enumerable.Empty<FormationDriver>(),
            };
        }
    }
}
