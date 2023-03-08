using Cardamom.Ui.Controller;

namespace SpaceOpera.Controller.Components
{
    public interface IActionController : IController
    {
        EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
    }
}
