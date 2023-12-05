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
    public class ArmyComponent : DynamicUiCompoundComponent, IFormationComponent
    {
        private static readonly string s_Container = "formation-pane-army-container";

        private static readonly DivisionSummaryComponent.Style s_DivisionStyle = new()
        {
            Container = "formation-pane-army-division-table",
            RowContainer =
                new()
                {
                    Container = "formation-pane-army-division-row",
                    ActionContainer = "formation-pane-army-division-row-action-container"
                },
            Icon = "formation-pane-army-division-row-icon",
            Info = "formation-pane-army-division-row-info",
            Text = "formation-pane-army-division-row-text",
            Status = "formation-pane-army-division-row-status-container",
            HealthText = "formation-pane-army-division-row-status-health-text",
            Health = "formation-pane-army-division-row-status-health",
            CohesionText =
                "formation-pane-army-division-row-status-cohesion-text",
            Cohesion = "formation-pane-army-division-row-status-cohesion"
        };
        private static readonly List<ActionRowStyles.ActionConfiguration> s_DivisionActions = new()
        {
            new()
            {
                Button = "formation-pane-army-division-row-action-split",
                Action = ActionId.Split
            }
        };

        public object Key => Driver;
        public ArmyDriver Driver { get; }
        public UiCompoundComponent Header { get; }
        public UiCompoundComponent CompositionTable { get; }

        public ArmyComponent(ArmyDriver driver, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new FormationComponentController(),
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Driver = driver;
            Header = new FormationComponentHeader(driver, uiElementFactory, iconFactory);
            Add(Header);

            CompositionTable = 
                DivisionSummaryComponent.Create(
                    driver.GetDivisions, s_DivisionActions, s_DivisionStyle, uiElementFactory, iconFactory);
            Add(CompositionTable);
        }
    }
}
