using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;
using SpaceOpera.Controller.Components;

namespace SpaceOpera.View.Game.Panes.Common
{
    public static class UnitGroupingSummaryComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public ActionRowStyles.Style? RowContainer { get; set; }
            public string? Icon { get; set; }
            public string? Info { get; set; }
            public string? TextContainer { get; set; }
            public string? Text { get; set; }
            public ChipSetStyles.ChipStyle? MilitaryPower { get; set; }
            public string? Count { get; set; }
            public string? Status { get; set; }
            public string? HealthText { get; set; }
            public string? Health { get; set; }
            public string? Shields { get; set; }
            public string? ShieldsText { get; set; }
        }

        private class UnitGroupingComponentFactory : IKeyedElementFactory<UnitGrouping>
        {
            private readonly List<ActionRowStyles.ActionConfiguration> _actions;
            private readonly Style _style;
            private readonly UiElementFactory _uiElementFactory;
            private readonly IconFactory _iconFactory;

            public UnitGroupingComponentFactory(
                IEnumerable<ActionRowStyles.ActionConfiguration> actions,
                Style style, 
                UiElementFactory uiElementFactory,
                IconFactory iconFactory)
            {
                _style = style;
                _actions = actions.ToList();
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
            }

            public IKeyedUiElement<UnitGrouping> Create(UnitGrouping unitGrouping)
            {
                return ActionRow<UnitGrouping>.Create(
                    unitGrouping,
                    ActionId.Unknown,
                    ActionId.Unknown,
                    _uiElementFactory,
                    _style.RowContainer!,
                    new List<IUiElement>()
                    {
                        _iconFactory.Create(
                            _uiElementFactory.GetClass(_style.Icon!), new InlayController(), unitGrouping),
                        new DynamicUiSerialContainer(
                            _uiElementFactory.GetClass(_style.Info!),
                            new NoOpElementController(),
                            UiSerialContainer.Orientation.Vertical)
                        {
                            new DynamicUiSerialContainer(
                                _uiElementFactory.GetClass(_style.TextContainer!),
                                new NoOpElementController(),
                                UiSerialContainer.Orientation.Horizontal)
                            {
                                new TextUiElement(
                                    _uiElementFactory.GetClass(_style.Text!),
                                    new InlayController(),
                                    unitGrouping.Unit.Name),
                                MilitaryPowerChip.Create(
                                    unitGrouping.GetMilitaryPower, _style.MilitaryPower!, _uiElementFactory),
                                new DynamicTextUiElement(
                                    _uiElementFactory.GetClass(_style.Count!),
                                    new InlayController(),
                                    () => unitGrouping.Count.ToString("N0")),
                            },
                            new DynamicUiSerialContainer(
                                _uiElementFactory.GetClass(_style.Status!),
                                new NoOpElementController(),
                                UiSerialContainer.Orientation.Vertical)
                            {
                                new DynamicTextUiElement(
                                    _uiElementFactory.GetClass(_style.HealthText!),
                                    new InlayController(),
                                    () => unitGrouping.Hitpoints.ToString("N0")),
                                new PoolBar(
                                    _uiElementFactory.GetClass(_style.Health!),
                                    new InlayController(),
                                    unitGrouping.Hitpoints),
                                new DynamicTextUiElement(
                                    _uiElementFactory.GetClass(_style.ShieldsText!),
                                    new InlayController(),
                                    () => unitGrouping.Shielding.ToString("N0")),
                                new PoolBar(
                                    _uiElementFactory.GetClass(_style.Shields!),
                                    new InlayController(),
                                    unitGrouping.Shielding)
                            }
                        }
                    },
                    _actions);
            }
        }

        public static DynamicUiCompoundComponent Create(
            KeyRange<UnitGrouping> range,
            IEnumerable<ActionRowStyles.ActionConfiguration> actions,
            Style style,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory)
        {
            return new DynamicUiCompoundComponent(
                new ActionComponentController(),
                DynamicKeyedContainer<UnitGrouping>.CreateSerial(
                    uiElementFactory.GetClass(style.Container!),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Vertical,
                    range,
                    new UnitGroupingComponentFactory(actions, style, uiElementFactory, iconFactory),
                    Comparer<UnitGrouping>.Create((x, y) => x.Unit.Name.CompareTo(y.Unit.Name))));
        }
    }
}
