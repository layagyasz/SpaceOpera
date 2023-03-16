using Cardamom.Ui;

namespace SpaceOpera.View.Panes
{
    public interface IGamePane : IUiContainer, IDynamic
    {
        EventHandler<EventArgs>? Populated { get; set; }

        IUiElement CloseButton { get; }
        void AddToBody(IUiElement element);
        void Populate(params object?[] args);
        void SetTitle(string title);
    }
}
