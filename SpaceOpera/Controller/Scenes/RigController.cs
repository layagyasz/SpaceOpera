using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Window;
using OpenTK.Windowing.Common;

namespace SpaceOpera.Controller.Scenes
{
    public class RigController : IActionController, IElementController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<MouseButtonClickEventArgs>? Clicked { get; set; }
        public EventHandler<EventArgs>? Focused { get; set; }

        private readonly IActionController[] _subControllers;

        public RigController(params IActionController[] subControllers)
        {
            _subControllers = subControllers;
            foreach (var subController in subControllers)
            {
                subController.Interacted += HandleInteraction;
            }
        }


        public void Bind(object @object) { }

        public void Unbind() { }

        public IUiElement Get()
        {
            throw new InvalidProgramException();
        }

        public virtual bool HandleKeyDown(KeyDownEventArgs e)
        {
            return false;
        }

        public virtual bool HandleTextEntered(TextEnteredEventArgs e)
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
            return false;
        }

        public virtual bool HandleMouseButtonDragged(MouseButtonDragEventArgs e)
        {
            return false;
        }

        public virtual bool HandleMouseWheelScrolled(MouseWheelEventArgs e)
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

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }
    }
}
