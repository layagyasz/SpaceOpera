using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.View.Overlay;

namespace SpaceOpera.Controller.Overlay
{
    public class OverlayButtonController : ButtonController, IOverlayController
    {
        public EventHandler<ElementEventArgs<OverlayButtonId>>? ButtonClicked { get; set; }

        private readonly OverlayButtonId _id;

        public OverlayButtonController(OverlayButtonId id)
        {
            _id = id;
        }

        public override bool HandleMouseButtonClicked(MouseButtonClickEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                ButtonClicked?.Invoke(this, new(_id));
            }
            return base.HandleMouseButtonClicked(e);
        }
    }
}
