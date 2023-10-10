using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes.DiplomacyPanes;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomacyComponent : UiCompoundComponent
    {
        private static readonly string s_Container = "diplomacy-pane-diplomacy-container";
        private static readonly string s_InfoContainer = "diplomacy-pane-diplomacy-info-container";

        public EventHandler<DiplomaticRelation>? Populated { get; set; }

        public DiplomaticAgreementSideComponent Left { get; }
        public DiplomaticAgreementSideComponent Right { get; }
        public DiplomaticAgreementComponent Agreement { get; }

        public DiplomacyComponent(UiElementFactory uiElementFactory)
            : base(
                  new DiplomacyComponentController(),
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController<UiSerialContainer>(),
                      UiSerialContainer.Orientation.Horizontal))
        {
            Left = new DiplomaticAgreementSideComponent(uiElementFactory);
            Right = new DiplomaticAgreementSideComponent(uiElementFactory);
            Agreement = new DiplomaticAgreementComponent(uiElementFactory);

            Add(Left);
            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_InfoContainer),
                    new TableController(0f),
                    UiSerialContainer.Orientation.Vertical) 
                { 
                    Agreement 
                });
            Add(Right);
        }

        public void Populate(DiplomaticRelation relation)
        {
            Populated?.Invoke(this, relation);
        }
    }
}
