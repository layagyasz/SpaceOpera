using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes.MilitaryPanes;
using SpaceOpera.Core;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Game.Panes.Common;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.MilitaryPanes
{
    public class ArmySummaryComponent
    {
        private static readonly string s_Container = "military-pane-formation-summary-army";
        private static readonly string s_SectionHeader = "military-pane-formation-summary-army-section-header";
        private static readonly DivisionSummaryComponent.Style s_DivisionStyle = new()
        {
            Container = "military-pane-formation-summary-army-division-table",
            RowContainer =
            new()
            {
                Container = "military-pane-formation-summary-army-division-row",
                ActionContainer = "military-pane-formation-summary-army-division-row-action-container"
            },
            Icon = "military-pane-formation-summary-army-division-row-icon",
            Info = "military-pane-formation-summary-army-division-row-info",
            TextContainer = "military-pane-formation-summary-army-division-row-text-container",
            Text = "military-pane-formation-summary-army-division-row-text",
            MilitaryPower = new()
            {
                Container = "military-pane-formation-summary-army-division-row-military-power",
                Icon = "military-pane-formation-summary-army-division-row-military-power-icon",
                Text = "military-pane-formation-summary-army-division-row-military-power-text"
            },
            Status = "military-pane-formation-summary-army-division-row-status-container",
            HealthText = "military-pane-formation-summary-army-division-row-status-health-text",
            Health = "military-pane-formation-summary-army-division-row-status-health",
            CohesionText =
                "military-pane-formation-summary-army-division-row-status-cohesion-text",
            Cohesion = "military-pane-formation-summary-army-division-row-status-cohesion"
        };
        private static readonly List<ActionRowStyles.ActionConfiguration> s_UnassignedDivisionActions = new()
        {
            new()
            {
                Button = "military-pane-formation-summary-army-division-row-action-join",
                Action = ActionId.Join
            }
        };
        private static readonly List<ActionRowStyles.ActionConfiguration> s_ComponentDivisionActions = new()
        {
            new()
            {
                Button = "military-pane-formation-summary-army-division-row-action-split",
                Action = ActionId.Split
            }
        };

        public static IUiComponent Create(
            World world,
            Faction faction,
            ArmyDriver driver, 
            UiElementFactory uiElementFactory, 
            IconFactory iconFactory)
        {
            return new DynamicUiCompoundComponent(
                new ArmySummaryController(driver),
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Container),
                    new NoOpElementController(), 
                    UiSerialContainer.Orientation.Vertical) 
                {
                    new TextUiElement(
                        uiElementFactory.GetClass(s_SectionHeader), new InlayController(), "Available Divisions"),
                    DivisionSummaryComponent.Create(
                        () => world.Formations.GetDivisionDriversFor(faction).Where(x => x.Parent == null),
                        s_UnassignedDivisionActions,
                        s_DivisionStyle,
                        uiElementFactory,
                        iconFactory),
                    new TextUiElement(
                        uiElementFactory.GetClass(s_SectionHeader), new InlayController(), "Assigned Divisions"),
                    DivisionSummaryComponent.Create(
                        driver.GetDivisions, 
                        s_ComponentDivisionActions,
                        s_DivisionStyle,
                        uiElementFactory, 
                        iconFactory)
                });
        }
    }
}
