using Cardamom.Ui.Controller;

namespace SpaceOpera.Controller
{
    public interface IActionController : IController
    {
        EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
    }
}
