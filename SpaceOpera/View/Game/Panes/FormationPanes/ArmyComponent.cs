using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Game.Panes.FormationPanes;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.FormationPanes
{
    public class ArmyComponent : DynamicUiCompoundComponent, IFormationComponent
    {
        private static readonly string s_Container = "formation-pane-army-container";

        private static readonly string s_DivisionTable = "formation-pane-army-division-table";
        private static readonly ActionRowStyles.Style s_DivisionRowStyle =
            new()
            {
                Container = "formation-pane-army-division-row",
                ActionContainer = "formation-pane-army-division-row-action-container"
            };
        private static readonly string s_DivisionIcon = "formation-pane-army-division-row-icon";
        private static readonly string s_DivisionInfo = "formation-pane-army-division-row-info";
        private static readonly string s_DivisionText = "formation-pane-army-division-row-text";
        private static readonly string s_DivisionStatus = "formation-pane-army-division-row-status-container";
        private static readonly string s_DivisionHealthText = "formation-pane-army-division-row-status-health-text";
        private static readonly string s_DivisionHealth = "formation-pane-army-division-row-status-health";
        private static readonly string s_DivisionCohesionText = 
            "formation-pane-army-division-row-status-cohesion-text";
        private static readonly string s_DivisionCohesion = "formation-pane-army-division-row-status-cohesion";
        private static readonly List<ActionRowStyles.ActionConfiguration> s_DivisionActions =
            new()
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
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    DynamicKeyedContainer<AtomicFormationDriver>.CreateSerial(
                        uiElementFactory.GetClass(s_DivisionTable),
                        new NoOpElementController(),
                        UiSerialContainer.Orientation.Vertical,
                        Driver.GetDivisions,
                        new SimpleKeyedElementFactory<AtomicFormationDriver>(uiElementFactory, iconFactory, CreateRow),
                        Comparer<AtomicFormationDriver>.Create(
                            (x, y) => x.Formation.Name.CompareTo(y.Formation.Name))));
            Add(CompositionTable);
        }

        private static ActionRow<AtomicFormationDriver> CreateRow(
            AtomicFormationDriver driver, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return ActionRow<AtomicFormationDriver>.Create(
                driver,
                ActionId.Unknown,
                ActionId.Unknown,
                uiElementFactory,
                s_DivisionRowStyle,
                new List<IUiElement>()
                {
                    iconFactory.Create(
                        uiElementFactory.GetClass(s_DivisionIcon), new InlayController(), driver.Formation),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_DivisionInfo),
                        new NoOpElementController(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new TextUiElement(
                            uiElementFactory.GetClass(s_DivisionText), new InlayController(), driver.Formation.Name),
                        new DynamicUiSerialContainer(
                            uiElementFactory.GetClass(s_DivisionStatus),
                            new NoOpElementController(),
                            UiSerialContainer.Orientation.Vertical)
                        {
                            new DynamicTextUiElement(
                                uiElementFactory.GetClass(s_DivisionHealthText),
                                new InlayController(),
                                () => driver.AtomicFormation.Health.ToString("N0")),
                            new PoolBar(
                                uiElementFactory.GetClass(s_DivisionHealth),
                                new InlayController(),
                                driver.AtomicFormation.Health),
                            new DynamicTextUiElement(
                                uiElementFactory.GetClass(s_DivisionCohesionText),
                                new InlayController(),
                                () => driver.AtomicFormation.Cohesion.ToString("P0")),
                            new PoolBar(
                                uiElementFactory.GetClass(s_DivisionCohesion),
                                new InlayController(),
                                driver.AtomicFormation.Cohesion)
                        }
                    }
                },
                s_DivisionActions);
        }
    }
}
