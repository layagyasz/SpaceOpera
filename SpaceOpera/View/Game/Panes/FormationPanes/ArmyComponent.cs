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
        private static readonly ActionRow<Division>.Style s_DivisionRowStyle =
            new()
            {
                Container = "formation-pane-army-division-row"
            };
        private static readonly string s_DivisionIcon = "formation-pane-army-division-row-icon";
        private static readonly string s_DivisionInfo = "formation-pane-army-division-row-info";
        private static readonly string s_DivisionText = "formation-pane-army-division-row-text";
        private static readonly string s_DivisionStatus = "formation-pane-army-division-row-status-container";
        private static readonly string s_DivisionHealthText = "formation-pane-army-division-row-status-health-text";
        private static readonly string s_DivisionHealth = "formation-pane-army-division-row-status-health";
        private static readonly string s_DivisionCohesionText = "formation-pane-army-division-row-status-cohesion-text";
        private static readonly string s_DivisionCohesion = "formation-pane-army-division-row-status-cohesion";

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
                    DynamicKeyedContainer<Division>.CreateSerial(
                        uiElementFactory.GetClass(s_DivisionTable),
                        new NoOpElementController(),
                        UiSerialContainer.Orientation.Vertical,
                        new FunctionRange<Division>(() => Driver.Army.Divisions),
                        new SimpleKeyedElementFactory<Division>(uiElementFactory, iconFactory, CreateRow),
                        Comparer<Division>.Create((x, y) => x.Name.CompareTo(y.Name))));
            Add(CompositionTable);
        }

        private static ActionRow<Division> CreateRow(
            Division division, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return ActionRow<Division>.Create(
                division,
                ActionId.Unknown,
                ActionId.Unknown,
                uiElementFactory,
                s_DivisionRowStyle,
                new List<IUiElement>()
                {
                    iconFactory.Create(uiElementFactory.GetClass(s_DivisionIcon), new InlayController(), division),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_DivisionInfo),
                        new NoOpElementController(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new TextUiElement(
                            uiElementFactory.GetClass(s_DivisionText), new InlayController(), division.Name),
                        new DynamicUiSerialContainer(
                            uiElementFactory.GetClass(s_DivisionStatus),
                            new NoOpElementController(),
                            UiSerialContainer.Orientation.Vertical)
                        {
                            new DynamicTextUiElement(
                                uiElementFactory.GetClass(s_DivisionHealthText),
                                new InlayController(),
                                () => division.Health.ToString("N0")),
                            new PoolBar(
                                uiElementFactory.GetClass(s_DivisionHealth),
                                new InlayController(),
                                division.Health),
                            new DynamicTextUiElement(
                                uiElementFactory.GetClass(s_DivisionCohesionText),
                                new InlayController(),
                                () => division.Cohesion.ToString("P0")),
                            new PoolBar(
                                uiElementFactory.GetClass(s_DivisionCohesion),
                                new InlayController(),
                                division.Cohesion)
                        }
                    }
                },
                Enumerable.Empty<ActionRow<Division>.ActionConfiguration>());
        }
    }
}
