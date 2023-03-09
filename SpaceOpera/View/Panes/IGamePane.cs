using Cardamom.Ui;

namespace SpaceOpera.View.Panes
{
    public interface IGamePane : IUiContainer, IDynamic
    {
        IUiElement CloseButton { get; }
        void AddToBody(IUiElement element);
        void Populate(params object?[] args);
        void SetTitle(string title);
    }
}
