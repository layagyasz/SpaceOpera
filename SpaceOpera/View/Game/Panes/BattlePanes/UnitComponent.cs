using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.BattlePanes
{
    public class UnitComponent : UiSerialContainer, IDynamic, IKeyedUiElement<Unit>
    {
        private static readonly string s_Class = "battle-pane-faction-unit-row";
        private static readonly string s_Icon = "battle-pane-faction-unit-row-icon";
        private static readonly string s_Info = "battle-pane-faction-unit-row-info";
        private static readonly string s_Text = "battle-pane-faction-unit-row-text";
        private static readonly string s_Stats = "battle-pane-faction-unit-row-stats";
        private static readonly string s_Output = "battle-pane-faction-unit-row-stats-output";
        private static readonly string s_Input = "battle-pane-faction-unit-row-stats-input";

        private static readonly string s_OutputRaw = "battle-pane-faction-unit-row-stats-damage-output-raw-icon";
        private static readonly string s_OutputOnTarget =
            "battle-pane-faction-unit-row-stats-damage-output-on-target-icon";
        private static readonly string s_OutputHull = "battle-pane-faction-unit-row-stats-damage-output-hull-icon";
        private static readonly string s_OutputEffective = 
            "battle-pane-faction-unit-row-stats-damage-output-effective-icon";
        private static readonly string s_InputRaw = "battle-pane-faction-unit-row-stats-damage-input-raw-icon";
        private static readonly string s_InputOnTarget = 
            "battle-pane-faction-unit-row-stats-damage-input-on-target-icon";
        private static readonly string s_InputHull = "battle-pane-faction-unit-row-stats-damage-input-hull-icon";
        private static readonly string s_InputEffective = 
            "battle-pane-faction-unit-row-stats-damage-input-effective-icon";

        public EventHandler<EventArgs>? Refreshed { get; set; }

        public Unit Key { get; }

        private readonly Faction _faction;
        private readonly ReportWrapper _report;

        private readonly TextUiElement _outputRaw;
        private readonly TextUiElement _outputOnTarget;
        private readonly TextUiElement _outputHull;
        private readonly TextUiElement _outputEffective;
        private readonly TextUiElement _inputRaw;
        private readonly TextUiElement _inputOnTarget;
        private readonly TextUiElement _inputHull;
        private readonly TextUiElement _inputEffective;

        public UnitComponent(
            Faction faction,
            Unit unit, 
            ReportWrapper report, 
            UiElementFactory uiElementFactory,
            IconFactory iconFactory)
            : base(uiElementFactory.GetClass(s_Class), new ButtonController(), Orientation.Horizontal)
        {
            _faction = faction;
            Key = unit;
            _report = report;

            Add(iconFactory.Create(uiElementFactory.GetClass(s_Icon), new InlayController(), unit));

            var output = uiElementFactory.GetClass(s_Output);
            _outputRaw = new(output, new InlayController(), string.Empty);
            _outputOnTarget = new(output, new InlayController(), string.Empty);
            _outputHull = new(output, new InlayController(), string.Empty);
            _outputEffective = new(output, new InlayController(), string.Empty);

            var input = uiElementFactory.GetClass(s_Input);
            _inputRaw = new(input, new InlayController(), string.Empty);
            _inputOnTarget = new(input, new InlayController(), string.Empty);
            _inputHull = new(input, new InlayController(), string.Empty);
            _inputEffective = new(input, new InlayController(), string.Empty);
            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_Info),
                    new NoOpElementController(),
                    Orientation.Vertical)
                {
                    new TextUiElement(uiElementFactory.GetClass(s_Text), new InlayController(), unit.Name),
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_Stats), new InlayController(), Orientation.Horizontal)
                    {
                        new SimpleUiElement(uiElementFactory.GetClass(s_OutputRaw), new InlayController()),
                        _outputRaw,
                        new SimpleUiElement(uiElementFactory.GetClass(s_OutputOnTarget), new InlayController()),
                        _outputOnTarget,
                        new SimpleUiElement(uiElementFactory.GetClass(s_OutputHull), new InlayController()),
                        _outputHull,
                        new SimpleUiElement(uiElementFactory.GetClass(s_OutputEffective), new InlayController()),
                        _outputEffective
                    },
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_Stats), new InlayController(), Orientation.Horizontal)
                    {
                        new SimpleUiElement(uiElementFactory.GetClass(s_InputRaw), new InlayController()),
                        _inputRaw,
                        new SimpleUiElement(uiElementFactory.GetClass(s_InputOnTarget), new InlayController()),
                        _inputOnTarget,
                        new SimpleUiElement(uiElementFactory.GetClass(s_InputHull), new InlayController()),
                        _inputHull,
                        new SimpleUiElement(uiElementFactory.GetClass(s_InputEffective), new InlayController()),
                        _inputEffective
                    }
                });
        }

        public void Refresh()
        {
            var report = _report.Report?.Get(_faction, Key)!;
            _outputRaw.SetText(report.TotalOutputRawDamage.ToString("N0"));
            _outputOnTarget.SetText(
                string.Format("{0:N0} ({1:P0})", report.TotalOutputOnTargetDamage, report.GetTargetingEfficiency()));
            _outputHull.SetText(
                string.Format(
                    "{0:N0} ({1:P0})", report.TotalOutputHullDamage, report.GetShieldPenetrationEfficiency()));
            _outputEffective.SetText(
                string.Format(
                    "{0:N0} ({1:P0})", report.TotalOutputEffectiveDamage, report.GetArmorPenetrationEfficiency()));
            _inputRaw.SetText(report.TotalInputRawDamage.ToString("N0"));
            _inputOnTarget.SetText(
                string.Format("{0:N0} ({1:P0})", report.TotalInputOnTargetDamage, report.GetManeuverEfficiency()));
            _inputHull.SetText(
                string.Format("{0:N0} ({1:P0})", report.TotalInputHullDamage, report.GetShieldEfficiency()));
            _inputEffective.SetText(
                string.Format("{0:N0} ({1:P0})", report.TotalInputEffectiveDamage, report.GetArmorEfficiency()));
            Refreshed?.Invoke(this, EventArgs.Empty);
        }
    }
}
