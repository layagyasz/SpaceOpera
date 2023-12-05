using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;
using SpaceOpera.Controller.Components;

namespace SpaceOpera.View.Game.Panes.Common
{
    public static class DivisionSummaryComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public ActionRowStyles.Style? RowContainer { get; set; }
            public string? Icon { get; set; }
            public string? Info { get; set; }
            public string? Text { get; set; }
            public string? Status { get; set; }
            public string? HealthText { get; set; }
            public string? Health { get; set; }
            public string? Cohesion { get; set; }
            public string? CohesionText { get; set; }
        }

        private class DivisionComponentFactory : IKeyedElementFactory<AtomicFormationDriver>
        {
            private readonly Style _style;
            private readonly List<ActionRowStyles.ActionConfiguration> _actions;
            private readonly UiElementFactory _uiElementFactory;
            private readonly IconFactory _iconFactory;

            public DivisionComponentFactory(
                IEnumerable<ActionRowStyles.ActionConfiguration> actions,
                Style style, 
                UiElementFactory uiElementFactory,
                IconFactory iconFactory)
            {
                _actions = actions.ToList();
                _style = style;
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
            }

            public IKeyedUiElement<AtomicFormationDriver> Create(AtomicFormationDriver driver)
            {
                return ActionRow<AtomicFormationDriver>.Create(
                    driver,
                    ActionId.Unknown,
                    ActionId.Unknown,
                    _uiElementFactory,
                    _style.RowContainer!,
                    new List<IUiElement>()
                    {
                        _iconFactory.Create(
                            _uiElementFactory.GetClass(_style.Icon!), new InlayController(), driver.Formation),
                        new DynamicUiSerialContainer(
                            _uiElementFactory.GetClass(_style.Info!),
                            new NoOpElementController(),
                            UiSerialContainer.Orientation.Vertical)
                        {
                            new TextUiElement(
                                _uiElementFactory.GetClass(_style.Text!),
                                new InlayController(), 
                                driver.Formation.Name),
                            new DynamicUiSerialContainer(
                                _uiElementFactory.GetClass(_style.Status!),
                                new NoOpElementController(),
                                UiSerialContainer.Orientation.Vertical)
                            {
                                new DynamicTextUiElement(
                                    _uiElementFactory.GetClass(_style.HealthText!),
                                    new InlayController(),
                                    () => driver.AtomicFormation.Health.ToString("N0")),
                                new PoolBar(
                                    _uiElementFactory.GetClass(_style.Health!),
                                    new InlayController(),
                                    driver.AtomicFormation.Health),
                                new DynamicTextUiElement(
                                    _uiElementFactory.GetClass(_style.CohesionText!),
                                    new InlayController(),
                                    () => driver.AtomicFormation.Cohesion.ToString("P0")),
                                new PoolBar(
                                    _uiElementFactory.GetClass(_style.Cohesion!),
                                    new InlayController(),
                                    driver.AtomicFormation.Cohesion)
                            }
                        }
                    },
                    _actions);
            }
        }

        public static DynamicUiCompoundComponent Create(
            KeyRange<AtomicFormationDriver> range,
            IEnumerable<ActionRowStyles.ActionConfiguration> actions,
            Style style,
            UiElementFactory uiElementFactory, 
            IconFactory iconFactory)
        {
            return new DynamicUiCompoundComponent(
                new ActionComponentController(),
                DynamicKeyedContainer<AtomicFormationDriver>.CreateSerial(
                    uiElementFactory.GetClass(style.Container!),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Vertical,
                    range,
                    new DivisionComponentFactory(actions, style, uiElementFactory, iconFactory),
                    Comparer<AtomicFormationDriver>.Create((x, y) => x.Formation.Name.CompareTo(y.Formation.Name))));
        }
    }
}
