using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomaticAgreementComponent : UiCompoundComponent
    {
        private static readonly string s_Container = "diplomacy-pane-diplomacy-agreement-container";

        private readonly UiElementFactory _uiElementFactory;

        public DiplomaticAgreementComponent(UiElementFactory uiElementFactory)
            : base(
                  new AdderComponentController<IDiplomaticAgreementSection>(), 
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new TableController(10f),
                      UiSerialContainer.Orientation.Horizontal))
        {
            _uiElementFactory = uiElementFactory;
        }

        public void SetAgreement(DiplomaticAgreement agreement) { }
    }
}
