using Cardamom.Ui.Controller;

namespace SpaceOpera.Controller
{
    public interface IPopupController : IController
    {
        EventHandler<PopupEventArgs>? PopupCreated { get; set; }
    }
}
