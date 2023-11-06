using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomaticAgreementSectionsComponent : UiCompoundComponent
    {
        private static readonly string s_Container = "diplomacy-pane-diplomacy-proposal-table";
        private static readonly string s_Section = "diplomacy-pane-diplomacy-proposal-section";
        private static readonly string s_SectionHeader = "diplomacy-pane-diplomacy-proposal-section-header";
        private static readonly string s_SectionContents = "diplomacy-pane-diplomacy-proposal-section-contents";

        private readonly UiElementFactory _uiElementFactory;

        public DiplomaticAgreementSectionsComponent(UiElementFactory uiElementFactory)
            : base(
                  new AdderComponentController<IDiplomaticAgreementSection>(), 
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_Container), 
                      new TableController(10f),
                      UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;
        }

        public void SetSections(IEnumerable<IDiplomaticAgreementSection> sections)
        {
            Clear(/* dispose= */ true);
            foreach (var section in sections)
            {
                var row = CreateRow(section);
                row.Initialize();
                Add(row);
            }
        }

        private IUiComponent CreateRow(IDiplomaticAgreementSection section)
        {
            var component = new UiCompoundComponent(
                new StaticAdderController<IDiplomaticAgreementSection>(section),
                new UiSerialContainer(
                    _uiElementFactory.GetClass(s_Section),
                    new NoOpElementController(), UiSerialContainer.Orientation.Vertical))
            {
                new TextUiElement(
                    _uiElementFactory.GetClass(s_SectionHeader), new InlayController(), section.Type.Name)
            };
            foreach (var content in GetContents(section))
            {
                component.Add(
                    new TextUiElement(_uiElementFactory.GetClass(s_SectionContents), new InlayController(), content));
            }
            return component;
        }

        private static IEnumerable<string> GetContents(IDiplomaticAgreementSection section)
        {
            if (section is TradeAgreement trade)
            {
                yield return trade.Trade.FromZone.Name;
                foreach (var material in trade.Trade.Materials)
                {
                    yield return $"{material.Value:N0} x {material.Key.Name}";
                }
            }
        }
    }
}
