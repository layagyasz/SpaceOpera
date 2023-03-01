using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Overlay;

namespace SpaceOpera.View.Overlay
{
    public class EmpireOverlay : UiGroupLayer, IUiLayer
    {
        private UiSerialContainer _container;

        private EmpireOverlay(IController controller, UiSerialContainer container)
            : base(controller)
        {
            _container = container;
            Add(container);
        }

        public IEnumerable<IUiElement> GetButtons()
        {
            return _container;
        }

        public static EmpireOverlay Create(UiElementFactory uiElementFactory)
        {
            return new(
                new EmpireOverlayController(),
                uiElementFactory.CreateTableRow(
                    "overlay-empire-container", 
                    new List<IUiElement>()
                    {
                        new SimpleUiElement(
                            uiElementFactory.GetClass("overlay-empire-research"),
                            new OverlayButtonController(OverlayButtonId.Research)),
                        new SimpleUiElement(
                            uiElementFactory.GetClass("overlay-empire-designer"),
                            new OverlayButtonController(OverlayButtonId.Design)),
                        new SimpleUiElement(
                            uiElementFactory.GetClass("overlay-empire-military"),
                            new OverlayButtonController(OverlayButtonId.Military))
                    }, 
                    new ButtonController()));
        }
    }
}
