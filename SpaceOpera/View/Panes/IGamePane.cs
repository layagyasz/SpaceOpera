using Cardamom.Ui;

namespace SpaceOpera.View.Panes
{
    public interface IGamePane : IUiContainer, IDynamic
    {
        EventHandler<EventArgs>? Populated { get; set; }

        IUiElement CloseButton { get; }
        void Populate(params object?[] args);
        void SetBody(IUiElement body);
        void SetTitle(string title);
    }
}
