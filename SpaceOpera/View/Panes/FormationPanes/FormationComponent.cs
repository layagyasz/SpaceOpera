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
    public class FormationComponent : DynamicUiCompoundComponent
    {
        private static readonly string s_ContainerClassName = "formation-pane-formation-container";
        private static readonly string s_UnitGroupingTableClassName = "formation-pane-formation-unit-grouping-table";

        private static readonly ActionRow<UnitGrouping>.Style s_UnitGroupingRowStyle =
            new() 
            {
                Container = "formation-pane-formation-unit-grouping-row"
            };
        private static readonly string s_UnitGroupingIconClassName = "formation-pane-formation-unit-grouping-row-icon";
        private static readonly string s_UnitGroupingInfoClassName = "formation-pane-formation-unit-grouping-row-info";
        private static readonly string s_UnitGroupingTextClassName = "formation-pane-formation-unit-grouping-row-text";
        private static readonly string s_UnitGroupingStatusClassName = 
            "formation-pane-formation-unit-grouping-row-status-container";
        private static readonly string s_UnitGroupingHealthClassName =
            "formation-pane-formation-unit-grouping-row-status-health";
        private static readonly string s_UnitGroupingShieldsClassName = 
            "formation-pane-formation-unit-grouping-row-status-shields";

        public FormationDriver Key { get; }
        public UiCompoundComponent Header { get; }
        public UiCompoundComponent UnitGroupingTable { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public FormationComponent(FormationDriver driver, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new FormationComponentController(), 
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(s_ContainerClassName),
                      new NoOpElementController<UiSerialContainer>(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Key = driver;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            Header = new FormationComponentHeader(driver, _uiElementFactory, _iconFactory);
            Add(Header);

            UnitGroupingTable =
                new DynamicUiCompoundComponent(
                    new ActionTableController(),
                    new DynamicKeyedTable<UnitGrouping, ActionRow<UnitGrouping>>(
                        uiElementFactory.GetClass(s_UnitGroupingTableClassName),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical,
                        GetRange,
                        CreateRow,
                        Comparer<UnitGrouping>.Create((x, y) => x.Unit.Name.CompareTo(y.Unit.Name))));
            Add(UnitGroupingTable);
        }

        private IEnumerable<UnitGrouping> GetRange()
        {
            return Key.Formation.Composition;
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
                        _uiElementFactory.GetClass(s_UnitGroupingIconClassName), new InlayController(), unitGrouping),
                    new DynamicUiSerialContainer(
                        _uiElementFactory.GetClass(s_UnitGroupingInfoClassName),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new TextUiElement(
                            _uiElementFactory.GetClass(s_UnitGroupingTextClassName),
                            new InlayController(),
                            unitGrouping.Unit.Name),
                        new DynamicUiSerialContainer(
                            _uiElementFactory.GetClass(s_UnitGroupingStatusClassName),
                            new NoOpElementController<UiSerialContainer>(), 
                            UiSerialContainer.Orientation.Vertical)
                        {
                            new PoolBar(
                                _uiElementFactory.GetClass(s_UnitGroupingHealthClassName),
                                new InlayController(), 
                                unitGrouping.Hitpoints),
                            new PoolBar(
                                _uiElementFactory.GetClass(s_UnitGroupingShieldsClassName),
                                new InlayController(), 
                                unitGrouping.Shielding)
                        }
                    }
                },
                Enumerable.Empty<ActionRow<UnitGrouping>.ActionConfiguration>());
        }
    }
}
