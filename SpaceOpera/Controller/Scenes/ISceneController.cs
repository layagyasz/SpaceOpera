using Cardamom.Ui.Controller.Element;

namespace SpaceOpera.Controller.Scenes
{
    public interface ISceneController : IElementController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
    }
}
