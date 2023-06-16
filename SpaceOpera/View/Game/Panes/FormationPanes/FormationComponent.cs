using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Game.Panes.FormationPanes;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.FormationPanes
{
    public class FormationComponent : DynamicUiCompoundComponent, IFormationComponent
    {
        private static readonly string s_Container = "formation-pane-formation-container";
        private static readonly string s_UnitGroupingTable = "formation-pane-formation-unit-grouping-table";

        private static readonly ActionRow<UnitGrouping>.Style s_UnitGroupingRowStyle =
            new() 
            {
                Container = "formation-pane-formation-unit-grouping-row"
            };
        private static readonly string s_UnitGroupingIcon = "formation-pane-formation-unit-grouping-row-icon";
        private static readonly string s_UnitGroupingInfo = "formation-pane-formation-unit-grouping-row-info";
        private static readonly string s_UnitGroupingTextContainer = 
            "formation-pane-formation-unit-grouping-row-text-container";
        private static readonly string s_UnitGroupingText = "formation-pane-formation-unit-grouping-row-text";
        private static readonly string s_UnitGroupingCount = "formation-pane-formation-unit-grouping-row-count";
        private static readonly string s_UnitGroupingStatus = 
            "formation-pane-formation-unit-grouping-row-status-container";
        private static readonly string s_UnitGroupingHealthText =
            "formation-pane-formation-unit-grouping-row-status-health-text";
        private static readonly string s_UnitGroupingHealth =
            "formation-pane-formation-unit-grouping-row-status-health";
        private static readonly string s_UnitGroupingShieldsText =
            "formation-pane-formation-unit-grouping-row-status-shields-text";
        private static readonly string s_UnitGroupingShields = 
            "formation-pane-formation-unit-grouping-row-status-shields";

        public object Key => Driver;
        public AtomicFormationDriver Driver { get; }
        public UiCompoundComponent Header { get; }
        public UiCompoundComponent CompositionTable { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public FormationComponent(AtomicFormationDriver driver, UiElementFactory uiElementFactory, IconFactory iconFactory)
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

            Header = new FormationComponentHeader(driver, _uiElementFactory, _iconFactory);
            Add(Header);

            CompositionTable =
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    new DynamicKeyedTable<UnitGrouping, ActionRow<UnitGrouping>>(
                        uiElementFactory.GetClass(s_UnitGroupingTable),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical,
                        GetRange,
                        CreateRow,
                        Comparer<UnitGrouping>.Create((x, y) => x.Unit.Name.CompareTo(y.Unit.Name))));
            Add(CompositionTable);
        }

        private IEnumerable<UnitGrouping> GetRange()
        {
            return Driver.AtomicFormation.Composition;
        }

        private ActionRow<UnitGrouping> CreateRow(UnitGrouping unitGrouping)
        {
            return ActionRow<UnitGrouping>.Create(
                unitGrouping,
                ActionId.Unknown,
                _uiElementFactory,
                s_UnitGroupingRowStyle,
                new List<IUiElement>()
                {
                    _iconFactory.Create(
                        _uiElementFactory.GetClass(s_UnitGroupingIcon), new InlayController(), unitGrouping),
                    new DynamicUiSerialContainer(
                        _uiElementFactory.GetClass(s_UnitGroupingInfo),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new DynamicUiSerialContainer(
                            _uiElementFactory.GetClass(s_UnitGroupingTextContainer),
                            new NoOpElementController<UiSerialContainer>(), 
                            UiSerialContainer.Orientation.Horizontal)
                        {
                            new TextUiElement(
                                _uiElementFactory.GetClass(s_UnitGroupingText),
                                new InlayController(),
                                unitGrouping.Unit.Name),
                            new DynamicTextUiElement(
                                _uiElementFactory.GetClass(s_UnitGroupingCount),
                                new InlayController(),
                                () => unitGrouping.Count.ToString("N0")),
                        },
                        new DynamicUiSerialContainer(
                            _uiElementFactory.GetClass(s_UnitGroupingStatus),
                            new NoOpElementController<UiSerialContainer>(), 
                            UiSerialContainer.Orientation.Vertical)
                        {
                            new DynamicTextUiElement(
                                _uiElementFactory.GetClass(s_UnitGroupingHealthText),
                                new InlayController(),
                                () => unitGrouping.Hitpoints.ToString("N0")),
                            new PoolBar(
                                _uiElementFactory.GetClass(s_UnitGroupingHealth),
                                new InlayController(), 
                                unitGrouping.Hitpoints),
                            new DynamicTextUiElement(
                                _uiElementFactory.GetClass(s_UnitGroupingShieldsText),
                                new InlayController(),
                                () => unitGrouping.Shielding.ToString("N0")),
                            new PoolBar(
                                _uiElementFactory.GetClass(s_UnitGroupingShields),
                                new InlayController(), 
                                unitGrouping.Shielding)
                        }
                    }
                },
                Enumerable.Empty<ActionRow<UnitGrouping>.ActionConfiguration>());
        }
    }
}
