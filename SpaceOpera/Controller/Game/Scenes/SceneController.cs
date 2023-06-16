using Cardamom.Ui.Controller.Element;
using Cardamom.Window;
using OpenTK.Windowing.Common;

namespace SpaceOpera.Controller.Game.Scenes
{
    public class SceneController : RigController
    {
        private readonly IElementController _cameraController;

        public SceneController(IElementController cameraController, params IActionController[] subControllers)
            : base(subControllers)
        {
            _cameraController = cameraController;
        }

        public override bool HandleFocusEntered()
        {
            Focused?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public override bool HandleFocusLeft()
        {
            return true;
        }

        public override bool HandleKeyDown(KeyDownEventArgs e)
        {
            return _cameraController.HandleKeyDown(e);
        }

        public override bool HandleTextEntered(TextEnteredEventArgs e)
        {
            Interacted?.Invoke(this, UiInteractionEventArgs.Create(Enumerable.Empty<object>(), e.Key));
            return true;
        }

        public override bool HandleMouseButtonDragged(MouseButtonDragEventArgs e)
        {
            return _cameraController.HandleMouseButtonDragged(e);
        }

        public override bool HandleMouseWheelScrolled(MouseWheelEventArgs e)
        {
            return _cameraController.HandleMouseWheelScrolled(e);
        }
    }
}
