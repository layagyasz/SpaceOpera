using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes.DiplomacyPanes;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomacyComponent : UiCompoundComponent
    {
        private static readonly string s_Container = "diplomacy-pane-diplomacy-container";
        private static readonly string s_InfoContainer = "diplomacy-pane-diplomacy-info-container";
        private static readonly string s_ProposalContainer = "diplomacy-pane-diplomacy-proposal-container";

        public EventHandler<DiplomaticRelation>? Populated { get; set; }

        public DiplomaticAgreementOptionsComponent LeftOptions { get; }
        public DiplomaticAgreementOptionsComponent RightOptions { get; }
        public DiplomaticAgreementSectionsComponent LeftSections { get; }
        public DiplomaticAgreementSectionsComponent RightSections { get; }

        public DiplomacyComponent(UiElementFactory uiElementFactory)
            : base(
                  new DiplomacyComponentController(),
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController<UiSerialContainer>(),
                      UiSerialContainer.Orientation.Horizontal))
        {
            LeftOptions = new DiplomaticAgreementOptionsComponent(uiElementFactory);
            RightOptions = new DiplomaticAgreementOptionsComponent(uiElementFactory);
            LeftSections = new DiplomaticAgreementSectionsComponent(uiElementFactory);
            RightSections = new DiplomaticAgreementSectionsComponent(uiElementFactory);

            Add(LeftOptions);
            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_InfoContainer),
                    new TableController(0f),
                    UiSerialContainer.Orientation.Vertical) 
                { 
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_ProposalContainer), 
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Horizontal)
                    {
                        LeftSections,
                        RightSections
                    }
                });
            Add(RightOptions);
        }

        public void Populate(DiplomaticRelation relation)
        {
            Populated?.Invoke(this, relation);
        }

        public void SetAgreement(DiplomaticAgreement agreement)
        {
            LeftSections.SetSections(agreement.Left);
            RightSections.SetSections(agreement.Right);
        }
    }
}
