using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components.Dynamics;

namespace SpaceOpera.View.Game.Panes.BattlePanes
{
    public class FactionComponent : DynamicUiSerialContainer, IKeyedUiElement<Faction>
    {
        private static readonly string s_Container = "battle-pane-faction-container";
        private static readonly string s_HeaderContainer = "battle-pane-faction-header";
        private static readonly string s_HeaderIcon = "battle-pane-faction-header-icon";
        private static readonly string s_HeaderText = "battle-pane-faction-header-text";
        private static readonly string s_UnitTable = "battle-pane-faction-unit-table";

        class UnitComponentFactory : IKeyedElementFactory<Unit>
        {
            private readonly UiElementFactory _uiElementFactory;
            private readonly IconFactory _iconFactory;
            private readonly Faction _faction;
            private readonly ReportWrapper _report;

            public UnitComponentFactory(
                UiElementFactory uiElementFactory, IconFactory iconFactory, Faction faction, ReportWrapper report)
            {
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
                _faction = faction;
                _report = report;
            }

            public IKeyedUiElement<Unit> Create(Unit unit)
            {
                return new UnitComponent(_faction, unit, _report, _uiElementFactory, _iconFactory);
            }
        }

        public Faction Key { get; }

        public FactionComponent(
            Faction faction, ReportWrapper report, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                uiElementFactory.GetClass(s_Container),
                new NoOpElementController(),
                Orientation.Vertical)
        {
            Key = faction;

            var header =
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_HeaderContainer), new ButtonController(), Orientation.Horizontal)
                {
                    iconFactory.Create(uiElementFactory.GetClass(s_HeaderIcon), new InlayController(), faction),
                    new TextUiElement(uiElementFactory.GetClass(s_HeaderText), new InlayController(), faction.Name)
                };
            Add(header);

            var units =
                DynamicKeyedContainer<Unit>.CreateSerial(
                    uiElementFactory.GetClass(s_UnitTable),
                    new NoOpElementController(),
                    Orientation.Vertical,
                    () => report.Report?.Get(faction).UnitReports.Select(x => x.Unit) ?? Enumerable.Empty<Unit>(),
                    new UnitComponentFactory(uiElementFactory, iconFactory, faction, report),
                    Comparer<Unit>.Create((x, y) => x.Name.CompareTo(y.Name)));
            Add(units);
        }
    }
}
