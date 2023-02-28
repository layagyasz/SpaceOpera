using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Overlay
{
    public class EmpireOverlay : UiSerialContainer, IUiLayer
    {
        public IUiElement ResearchButton { get; }
        public IUiElement DesignerButton { get; }
        public IUiElement MilitaryButton { get; }
        
        private EmpireOverlay(
            Class @class,
            IElementController controller,
            IUiElement researchButton,
            IUiElement designerButton, 
            IUiElement militaryButton)
            : base(@class, controller, Orientation.Horizontal)
        {
            ResearchButton = researchButton;
            Add(researchButton);
            DesignerButton = designerButton;
            Add(designerButton);
            MilitaryButton = militaryButton;
            Add(militaryButton);
        }

        public static EmpireOverlay Create(UiElementFactory uiElementFactory)
        {
            return new(
                uiElementFactory.GetClass("overlay-empire-container"),
                new ButtonController(),
                uiElementFactory.CreateSimpleButton("overlay-empire-research", new()).Item1,
                uiElementFactory.CreateSimpleButton("overlay-empire-designer", new()).Item1,
                uiElementFactory.CreateSimpleButton("overlay-empire-military", new()).Item1);
        }
    }
}
