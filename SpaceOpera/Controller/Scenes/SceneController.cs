using Cardamom.Ui.Controller.Element;
using Cardamom.Window;
using OpenTK.Windowing.Common;

namespace SpaceOpera.Controller.Scenes
{
    public class SceneController : RigController
    {
        private readonly IElementController _cameraController;

        public SceneController(IElementController cameraController, params ISceneController[] subControllers)
            : base(subControllers)
        {
            _cameraController = cameraController;
        }

        public override bool HandleKeyDown(KeyDownEventArgs e)
        {
            return _cameraController.HandleKeyDown(e);
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
