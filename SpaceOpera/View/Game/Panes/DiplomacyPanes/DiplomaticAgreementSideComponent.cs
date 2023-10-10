using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomaticAgreementSideComponent : UiCompoundComponent
    {
        private static readonly string s_Container = "diplomacy-pane-diplomacy-side-container";

        private readonly UiElementFactory _uiElementFactory;

        public DiplomaticAgreementSideComponent(UiElementFactory uiElementFactory)
            : base(
                  new ActionComponentController(), 
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new TableController(10f),
                      UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;
        }

        public void SetRange(IEnumerable<DiplomacyType> range) { }
    }
}
