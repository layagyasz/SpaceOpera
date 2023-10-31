using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Game.Panes.DiplomacyPanes;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomaticAgreementSectionsComponent : UiCompoundComponent
    {
        private static readonly string s_Container = "diplomacy-pane-diplomacy-proposal-table";
        private static readonly string s_Section = "diplomacy-pane-diplomacy-proposal-section";

        private static readonly string s_DefensePact = "Defense Pact";
        private static readonly string s_PeaceAgreement = "Peace Treaty";
        private static readonly string s_WarDeclaration = "Declare War";

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
            if (section is DefensePact)
            {
                return new UiSimpleComponent(
                    new SimpleSectionComponentController(section),
                    _uiElementFactory.CreateTextButton(s_Section, s_DefensePact).Item1);
            }
            if (section is PeaceProposal)
            {
                return new UiSimpleComponent(
                    new SimpleSectionComponentController(section),
                    _uiElementFactory.CreateTextButton(s_Section, s_PeaceAgreement).Item1);
            }
            if (section is WarDeclaration)
            {
                return new UiSimpleComponent(
                    new SimpleSectionComponentController(section),
                    _uiElementFactory.CreateTextButton(s_Section, s_WarDeclaration).Item1);
            }
            throw new ArgumentException($"Unsupported DiplomacyType: [{section.Type}]");
        }
    }
}
