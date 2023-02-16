
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Window;
using OpenTK.Windowing.Common;

namespace SpaceOpera.Controller.Scenes
{
    public class SceneController : ISceneController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<MouseButtonClickEventArgs>? Clicked { get; set; }
        public EventHandler<EventArgs>? Focused { get; set; }

        private readonly IElementController _cameraController;
        private readonly ISceneController[] _subControllers;

        public SceneController(IElementController cameraController, params ISceneController[] subControllers)
        {
            _cameraController = cameraController;
            _subControllers = subControllers;
            foreach (var subController in subControllers)
            {
                subController.Interacted += HandleInteraction;
            }
        }


        public void Bind(object @object) { }

        public void Unbind() { }

        public bool HandleKeyDown(KeyDownEventArgs e)
        {
            return _cameraController.HandleKeyDown(e);
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
            return false;
        }

        public bool HandleMouseButtonDragged(MouseButtonDragEventArgs e)
        {
            return _cameraController.HandleMouseButtonDragged(e);
        }

        public bool HandleMouseWheelScrolled(MouseWheelEventArgs e)
        {
            return _cameraController.HandleMouseWheelScrolled(e);
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
