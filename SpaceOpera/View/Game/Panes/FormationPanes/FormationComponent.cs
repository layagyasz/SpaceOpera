using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes.FormationPanes;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Game.Panes.Common;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.FormationPanes
{
    public class FormationComponent : DynamicUiCompoundComponent, IFormationComponent
    {
        private static readonly string s_Container = "formation-pane-formation-container";
        private static readonly UnitGroupingSummaryComponent.Style s_UnitGroupingStyle = new()
        {
            Container = "formation-pane-formation-unit-grouping-table",
            RowContainer = new()
            {
                Container = "formation-pane-formation-unit-grouping-row"
            },
            Icon = "formation-pane-formation-unit-grouping-row-icon",
            Info = "formation-pane-formation-unit-grouping-row-info",
            TextContainer = "formation-pane-formation-unit-grouping-row-text-container",
            Text = "formation-pane-formation-unit-grouping-row-text",
            Count = "formation-pane-formation-unit-grouping-row-count",
            Status = "formation-pane-formation-unit-grouping-row-status-container",
            HealthText = "formation-pane-formation-unit-grouping-row-status-health-text",
            Health = "formation-pane-formation-unit-grouping-row-status-health",
            ShieldsText = "formation-pane-formation-unit-grouping-row-status-shields-text",
            Shields = "formation-pane-formation-unit-grouping-row-status-shields"
        };
        private static readonly List<ActionRowStyles.ActionConfiguration> s_UnitGroupingActions = new();

        public object Key => Driver;
        public AtomicFormationDriver Driver { get; }
        public UiCompoundComponent Header { get; }
        public UiCompoundComponent CompositionTable { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public FormationComponent(
            AtomicFormationDriver driver, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new FormationComponentController(), 
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Driver = driver;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            Header = new FormationComponentHeader(driver, _uiElementFactory, _iconFactory);
            Add(Header);

            CompositionTable = 
                UnitGroupingSummaryComponent.Create(
                    () => driver.AtomicFormation.Composition, 
                    s_UnitGroupingActions,
                    s_UnitGroupingStyle, 
                    uiElementFactory,
                    iconFactory);
            Add(CompositionTable);
        }
    }
}
