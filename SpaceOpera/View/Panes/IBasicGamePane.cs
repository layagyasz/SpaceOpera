using Cardamom.Ui;

namespace SpaceOpera.View.Panes
{
    public interface IBasicGamePane : IGamePane
    {
        IUiElement CloseButton { get; }
    }
}
