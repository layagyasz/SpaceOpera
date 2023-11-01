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
            return new UiSimpleComponent(
                new StaticAdderController<IDiplomaticAgreementSection>(section),
                _uiElementFactory.CreateTextButton(s_Section, section.Type.Name).Item1);
        }
    }
}
