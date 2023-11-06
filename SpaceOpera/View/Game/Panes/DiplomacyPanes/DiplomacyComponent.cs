using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes.DiplomacyPanes;
using SpaceOpera.Core;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomacyComponent : UiCompoundComponent
    {
        public record struct PopulatedEventArgs(World World, DiplomaticRelation Relation);

        private static readonly string s_Container = "diplomacy-pane-diplomacy-container";
        private static readonly string s_InfoContainer = "diplomacy-pane-diplomacy-info-container";
        private static readonly string s_ProposalContainer = "diplomacy-pane-diplomacy-proposal-container";
        private static readonly string s_Image = "diplomacy-pane-diplomacy-image";
        private static readonly string s_Submit = "diplomacy-pane-diplomacy-submit";

        public EventHandler<PopulatedEventArgs>? Populated { get; set; }

        public DiplomaticAgreementOptionsComponent LeftOptions { get; }
        public DiplomaticAgreementOptionsComponent RightOptions { get; }
        public DiplomaticAgreementSectionsComponent LeftSections { get; }
        public DiplomaticAgreementSectionsComponent RightSections { get; }
        public IUiElement Image { get; }
        public IUiElement Submit { get; }

        public DiplomacyComponent(UiElementFactory uiElementFactory)
            : base(
                  new DiplomacyComponentController(),
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Horizontal))
        {
            LeftOptions = new DiplomaticAgreementOptionsComponent(/* IsLeft= */ true, "We Provide", uiElementFactory);
            RightOptions = 
                new DiplomaticAgreementOptionsComponent(/* IsLeft= */ false, "They Provide", uiElementFactory);
            LeftSections = new DiplomaticAgreementSectionsComponent(uiElementFactory);
            RightSections = new DiplomaticAgreementSectionsComponent(uiElementFactory);
            Image =
                new SimpleUiElement(uiElementFactory.GetClass(s_Image), new NoOpElementController());
            Submit = uiElementFactory.CreateTextButton(s_Submit, "Submit").Item1;

            Add(LeftOptions);
            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_InfoContainer),
                    new TableController(0f),
                    UiSerialContainer.Orientation.Vertical) 
                { 
                    Image,
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_ProposalContainer), 
                        new NoOpElementController(),
                        UiSerialContainer.Orientation.Horizontal)
                    {
                        LeftSections,
                        RightSections
                    },
                    Submit
                });
            Add(RightOptions);
        }

        public void Populate(World world, DiplomaticRelation relation)
        {
            Populated?.Invoke(this, new(world, relation));
        }

        public void SetAgreement(DiplomaticAgreement agreement)
        {
            LeftSections.SetSections(agreement.Left);
            RightSections.SetSections(agreement.Right);
        }
    }
}
