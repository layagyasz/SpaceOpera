using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Window;
using OpenTK.Windowing.Common;

namespace SpaceOpera.Controller.Scenes
{
    public class SubRegionController : IActionController, IElementController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<MouseButtonClickEventArgs>? Clicked { get; set; }
        public EventHandler<EventArgs>? Focused { get; set; }

        private readonly object _subRegion;

        public SubRegionController(object subRegion)
        {
            _subRegion = subRegion;
        }

        public void Bind(object @object) { }

        public void Unbind() { }

        public IUiElement Get()
        {
            throw new InvalidProgramException();
        }

        public bool HandleKeyDown(KeyDownEventArgs e)
        {
            return false;
        }

        public bool HandleTextEntered(TextEnteredEventArgs e)
        {
            return false;
        }

        public bool HandleMouseEntered()
        {
            return false;
        }

        public bool HandleMouseLeft()
        {
            return false;
        }

        public bool HandleMouseButtonClicked(MouseButtonClickEventArgs e)
        {
            Clicked?.Invoke(this, e);
            Interacted?.Invoke(this, UiInteractionEventArgs.Create(_subRegion, e.Button));
            return true;
        }

        public bool HandleMouseButtonDragged(MouseButtonDragEventArgs e)
        {
            return false;
        }

        public bool HandleMouseWheelScrolled(MouseWheelEventArgs e)
        {
            return false;
        }

        public bool HandleMouseLingered()
        {
            return false;
        }

        public bool HandleMouseLingerBroken()
        {
            return false;
        }

        public bool HandleFocusEntered()
        {
            return false;
        }

        public bool HandleFocusLeft()
        {
            return false;
        }
    }
}
