using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Panes.FormationPanes;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.FormationPanes
{
    public class ArmyComponent : DynamicUiCompoundComponent, IFormationComponent
    {
        private static readonly string s_Container = "formation-pane-army-container";

        private static readonly string s_DivisionTable = "formation-pane-army-division-table";
        private static readonly ActionRow<Division>.Style s_DivisionRowStyle =
            new()
            {
                Container = "formation-pane-army-division-row"
            };
        private static readonly string s_DivisionIcon = "formation-pane-army-division-row-icon";
        private static readonly string s_DivisionText = "formation-pane-army-division-row-text";

        public object Key => Driver;
        public ArmyDriver Driver { get; }
        public UiCompoundComponent Header { get; }
        public UiCompoundComponent CompositionTable { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public ArmyComponent(ArmyDriver driver, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new FormationComponentController(),
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController<UiSerialContainer>(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Driver = driver;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            Header = new FormationComponentHeader(driver, uiElementFactory, iconFactory);
            Add(Header);

            CompositionTable =
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    new DynamicKeyedTable<Division, ActionRow<Division>>(
                        uiElementFactory.GetClass(s_DivisionTable),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical,
                        GetRange,
                        CreateRow,
                        Comparer<Division>.Create((x, y) => x.Name.CompareTo(y.Name))));
            Add(CompositionTable);
        }

        private IEnumerable<Division> GetRange()
        {
            return Driver.Army.Divisions;
        }

        private ActionRow<Division> CreateRow(Division division)
        {
            return ActionRow<Division>.Create(
                division,
                ActionId.Unknown,
                _uiElementFactory,
                s_DivisionRowStyle,
                new List<IUiElement>()
                {
                    _iconFactory.Create(_uiElementFactory.GetClass(s_DivisionIcon), new InlayController(), division),
                    new TextUiElement(_uiElementFactory.GetClass(s_DivisionText), new InlayController(), division.Name)
                },
                Enumerable.Empty<ActionRow<Division>.ActionConfiguration>());
        }
    }
}
