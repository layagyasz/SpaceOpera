using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Game.Panes.Common;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.MilitaryPanes
{
    public class FleetSummaryComponent
    {
        private static readonly string s_Container = "military-pane-formation-summary-fleet";
        private static readonly string s_SectionHeader = "military-pane-formation-summary-fleet-section-header";
        private static readonly UnitGroupingSummaryComponent.Style s_UnitGroupingStyle = new()
        {
            Container = "military-pane-formation-summary-fleet-unit-grouping-table",
            RowContainer =
            new()
            {
                Container = "military-pane-formation-summary-fleet-unit-grouping-row",
                ActionContainer = "military-pane-formation-summary-fleet-unit-grouping-row-action-container"
            },
            Icon = "military-pane-formation-summary-fleet-unit-grouping-row-icon",
            Info = "military-pane-formation-summary-fleet-unit-grouping-row-info",
            TextContainer = "military-pane-formation-summary-fleet-unit-grouping-row-text-container",
            Text = "military-pane-formation-summary-fleet-unit-grouping-row-text",
            Count = "military-pane-formation-summary-fleet-unit-grouping-row-count",
            Status = "military-pane-formation-summary-fleet-unit-grouping-row-status-container",
            HealthText = "military-pane-formation-summary-fleet-unit-grouping-row-status-health-text",
            Health = "military-pane-formation-summary-fleet-unit-grouping-row-status-health",
            ShieldsText =
                "military-pane-formation-summary-fleet-unit-grouping-row-status-shields-text",
            Shields = "military-pane-formation-summary-fleet-unit-grouping-row-status-shields"
        };
        private static readonly List<ActionRowStyles.ActionConfiguration> s_UnitGroupingActions = new();

        public static IUiComponent Create(
            FleetDriver driver,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory)
        {
            return new DynamicUiCompoundComponent(
                new TabComponentController(),
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Container),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    new TextUiElement(
                        uiElementFactory.GetClass(s_SectionHeader), new InlayController(), "Composition"),
                    UnitGroupingSummaryComponent.Create(
                        () => driver.AtomicFormation.Composition,
                        s_UnitGroupingActions,
                        s_UnitGroupingStyle,
                        uiElementFactory,
                        iconFactory)
                });
        }
    }
}
