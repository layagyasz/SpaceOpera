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

        private static readonly ActionRow<IFormation>.Style s_HeaderStyle =
            new() 
            {
                Container = "formation-pane-formation-header",
                Icon = "formation-pane-formation-header-icon",
                Text = "formation-pane-formation-header-text",
                ActionContainer = "formation-pane-formation-header-action-container"
            };
        private static readonly List<ActionRow<IFormation>.ActionConfiguration> s_HeaderActions =
            new()
            { 
                new ()
                {
                    Button = "formation-pane-formation-header-action-close",
                    Action = ActionId.Close
                }
            };

        private static readonly ActionRow<UnitGrouping>.Style s_UnitGroupingRowStyle =
            new() 
            {
                Container = "formation-pane-formation-unit-grouping-row",
                Icon = "formation-pane-formation-unit-grouping-row-icon",
                Text = "formation-pane-formation-unit-grouping-row-text"
            };

        public IFormation Key { get; }
        public UiCompoundComponent Header { get; }
        public UiCompoundComponent UnitGroupingTable { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public FormationComponent(IFormation formation, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new FormationComponentController(), 
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(s_ContainerClassName),
                      new NoOpElementController<UiSerialContainer>(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Key = formation;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            Header = 
                ActionRow<IFormation>.Create(
                    formation, formation.Name, uiElementFactory, iconFactory, s_HeaderStyle, s_HeaderActions);
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
            return Key.Composition;
        }

        private ActionRow<UnitGrouping> CreateRow(UnitGrouping unitGrouping)
        {
            return ActionRow<UnitGrouping>.Create(
                unitGrouping, 
                unitGrouping.Unit.Name,
                _uiElementFactory,
                _iconFactory,
                s_UnitGroupingRowStyle,
                Enumerable.Empty<ActionRow<UnitGrouping>.ActionConfiguration>());
        }
    }
}
