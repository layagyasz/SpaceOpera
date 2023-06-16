using Cardamom.Ui;

namespace SpaceOpera.View.Game.Overlay
{
    public interface IOverlay : IUiElement, IDynamic
    {
        void Populate(params object?[] args);
    }
}
