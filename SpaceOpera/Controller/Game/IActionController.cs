using Cardamom.Ui.Controller;

namespace SpaceOpera.Controller.Game
{
    public interface IActionController : IController
    {
        EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
    }
}
