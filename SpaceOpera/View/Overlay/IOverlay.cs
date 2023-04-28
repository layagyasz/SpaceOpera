using Cardamom.Ui;

namespace SpaceOpera.View.Overlay
{
    public interface IOverlay : IUiElement, IDynamic
    {
        void Populate(params object?[] args);
    }
}
