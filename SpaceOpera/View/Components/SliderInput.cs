using Cardamom.Mathematics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;

namespace SpaceOpera.View.Components
{
    public class SliderInput : UiRootComponent
    {
        public class Style
        {
            public string? Track { get; set; }
            public string? Knob { get; set; }
        }

        public IUiElement Knob { get; }

        private SliderInput(IController controller, IUiElement track, IUiElement knob)
            : base(controller, track, /* internalAnchor= */ true)
        {
            Knob = knob;
            Add(knob);
        }

        public static SliderInput Create(IntInterval range, UiElementFactory uiElementFactory, Style style)
        {
            return new(
                new SliderInputController(range),
                new SimpleUiElement(uiElementFactory.GetClass(style.Track!), new InlayController()), 
                new SimpleUiElement(uiElementFactory.GetClass(style.Knob!), new SliderInputKnobController()));
        }
    }
}
