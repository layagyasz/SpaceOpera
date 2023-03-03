﻿using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Overlay;

namespace SpaceOpera.View.Overlay
{
    public class EmpireOverlay : UiCompoundComponent
    {
        private EmpireOverlay(IController controller, UiSerialContainer container)
            : base(controller, container) { }

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
