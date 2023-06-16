using Cardamom.Ui;

namespace SpaceOpera.View.Game.Panes
{
    public interface IBasicGamePane : IGamePane
    {
        IUiElement CloseButton { get; }
    }
}
