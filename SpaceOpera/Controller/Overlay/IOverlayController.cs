using Cardamom.Ui.Controller;
using SpaceOpera.View.Overlay;

namespace SpaceOpera.Controller.Overlay
{
    public interface IOverlayController : IController
    {
        EventHandler<ElementEventArgs<OverlayButtonId>>? ButtonClicked { get; set; }
    }
}
