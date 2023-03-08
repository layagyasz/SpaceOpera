using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.View;

namespace SpaceOpera.Controller.Components
{
    public class ActionButtonController : ButtonController, IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        private readonly ActionId _id;

        public ActionButtonController(ActionId id)
        {
            _id = id;
        }

        public override bool HandleMouseButtonClicked(MouseButtonClickEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                Interacted?.Invoke(this, UiInteractionEventArgs.Create(Enumerable.Empty<object>(), _id));
            }
            return base.HandleMouseButtonClicked(e);
        }
    }
}
